using AtmApplication.Backend.ApplicationConstants;
using AtmApplication.Backend.DTOs;
using AtmApplication.DataAccess.Interfaces;
using AtmApplication.Backend.Exceptions;
using AtmApplication.DataAccess;
using AtmApplication.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtmApplication.Backend.Services
{
    public sealed class TransactionService : ITransactionService
    {
        private readonly IValidationService _validationService;
        private readonly IAccountRepository _accountRepository;
        private readonly IAtmRepository _atmRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(
            IValidationService validationService,
            IAccountRepository accountRepository,
            IAtmRepository atmRepository,
            ITransactionRepository transactionRepository)
        {
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _atmRepository = atmRepository ?? throw new ArgumentNullException(nameof(atmRepository));
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        }

        public async Task<TransactionResponseDto> DepositAsync(TransactionDto transactionDto, int pin)
        {
            // Validate account is not frozen
            await _validationService.ValidateAccountNotFrozenAsync(transactionDto.Username);

            // Verify PIN
            var pinVerification = await _validationService.VerifyPinWithAttemptsAsync(transactionDto.Username, pin);
            if (!pinVerification.IsVerified)
            {
                throw new InvalidCredentialsException();
            }

            var account = await _accountRepository.GetDataByUsernameAsync(transactionDto.Username);
            if (account == null)
            {
                throw new InvalidOperationException("Account not found.");
            }

            // Update user balance
            account.Balance += transactionDto.Amount;
            await _accountRepository.UpdateDataAsync(account);

            // Update ATM balance
            var atmDetails = (await _atmRepository.GetAllDataAsync()).FirstOrDefault();
            if (atmDetails != null)
            {
                atmDetails.TotalBalance += transactionDto.Amount;
                await _atmRepository.UpdateDataAsync(atmDetails);
            }

            // Record transaction
            var transaction = new TransactionDetails
            {
                UserName = transactionDto.Username,
                Type = transactionDto.Type,
                Amount = transactionDto.Amount,
                TimeStamp = DateTime.UtcNow,
                NewBalance = account.Balance,
                IsAdminTransaction = false
            };

            await _transactionRepository.AddDataAsync(transaction);

            return new TransactionResponseDto
            {
                Username = transactionDto.Username,
                Type = transactionDto.Type,
                Amount = transactionDto.Amount,
                NewBalance = account.Balance,
                Timestamp = transaction.TimeStamp
            };
        }

        public async Task<TransactionResponseDto> WithdrawAsync(TransactionDto transactionDto, int pin)
        {
            // Validate account is not frozen
            await _validationService.ValidateAccountNotFrozenAsync(transactionDto.Username);

            // Verify PIN
            var pinVerification = await _validationService.VerifyPinWithAttemptsAsync(transactionDto.Username, pin);
            if (!pinVerification.IsVerified)
            {
                throw new InvalidCredentialsException();
            }

            var account = await _accountRepository.GetDataByUsernameAsync(transactionDto.Username);
            if (account == null)
            {
                throw new InvalidOperationException(ExceptionMessages.AccountNotFound);
            }

            // Check user balance
            if (account.Balance < transactionDto.Amount)
            {
                throw new InsufficientFundsException();
            }

            // Check ATM balance
            var atmDetails = (await _atmRepository.GetAllDataAsync()).FirstOrDefault();
            if (atmDetails == null)
            {
                throw new InvalidOperationException("ATM details not found.");
            }
            
            if (atmDetails.TotalBalance < transactionDto.Amount)
            {
                throw new InvalidOperationException("ATM does not have sufficient cash. Please try a smaller amount.");
            }

            // Update user balance
            account.Balance -= transactionDto.Amount;
            await _accountRepository.UpdateDataAsync(account);

            // Update ATM balance
            atmDetails.TotalBalance -= transactionDto.Amount;
            await _atmRepository.UpdateDataAsync(atmDetails);

            // Record transaction
            var transaction = new TransactionDetails
            {
                UserName = transactionDto.Username,
                Type = transactionDto.Type,
                Amount = transactionDto.Amount,
                TimeStamp = DateTime.UtcNow,
                NewBalance = account.Balance,
                IsAdminTransaction = false
            };

            await _transactionRepository.AddDataAsync(transaction);

            return new TransactionResponseDto
            {
                Username = transactionDto.Username,
                Type = transactionDto.Type,
                Amount = transactionDto.Amount,
                NewBalance = account.Balance,
                Timestamp = transaction.TimeStamp
            };
        }

        public async Task<TransactionsHistoryDto> GetTransactionHistoryAsync(string username, int count)
        {
            // Check if user is admin by comparing with ATM's current admin
            var atmDetails = (await _atmRepository.GetAllDataAsync()).FirstOrDefault();
            bool isAdmin = atmDetails != null && atmDetails.AdminUsername.Equals(username);

            var allTransactions = await _transactionRepository.GetAllDataAsync();
            
            IEnumerable<TransactionDetails> filteredTransactions;
            
            if (isAdmin)
            {
                // Admin sees ALL transactions from ALL users
                filteredTransactions = allTransactions
                    .OrderByDescending(t => t.TimeStamp)
                    .Take(count);
            }
            else
            {
                // Regular user sees only their own transactions
                filteredTransactions = allTransactions
                    .Where(t => t.UserName.Equals(username))
                    .OrderByDescending(t => t.TimeStamp)
                    .Take(count);
            }

            var transactionList = filteredTransactions
                .Select(t => new TransactionHistoryItemDto
                {
                    Username = t.UserName,
                    Type = t.Type,
                    Amount = t.Amount,
                    NewBalance = t.NewBalance,
                    Timestamp = t.TimeStamp,
                    IsAdminTransaction = t.IsAdminTransaction
                })
                .ToList();

            return new TransactionsHistoryDto
            {
                Transactions = transactionList
            };
        }

        public async Task<BalanceDto> GetBalanceAsync(string username, int pin)
        {
            // Validate account is not frozen
            await _validationService.ValidateAccountNotFrozenAsync(username);

            // Verify PIN
            var pinVerification = await _validationService.VerifyPinWithAttemptsAsync(username, pin);
            if (!pinVerification.IsVerified)
            {
                throw new InvalidCredentialsException();
            }

            var account = await _accountRepository.GetDataByUsernameAsync(username);
            if (account == null)
            {
                throw new InvalidOperationException(
                    string.Format(ExceptionMessages.AccountNotFound, username));
            }

            return new BalanceDto
            {
                Username = username,
                Balance = account.Balance
            };
        }

        public async Task<AtmBalanceDto> GetAtmBalanceAsync(string adminUsername)
        {
            // Validate admin
            await _validationService.ValidateAdminAsync(adminUsername);

            var atmDetails = (await _atmRepository.GetAllDataAsync()).FirstOrDefault();

            return new AtmBalanceDto
            {
                TotalBalance = atmDetails != null ? atmDetails.TotalBalance : 0
            };
        }

        public async Task<bool> DepositToAtmAsync(DepositCashDto dto)
        {
            // Validate admin
            await _validationService.ValidateAdminAsync(dto.AdminUsername);

            var atmDetails = (await _atmRepository.GetAllDataAsync()).FirstOrDefault();
            
            if (atmDetails == null)
            {
                throw new InvalidOperationException(ExceptionMessages.AtmNotFound);
            }

            // Update balance and current admin
            atmDetails.TotalBalance += dto.Amount;
            atmDetails.AdminUsername = dto.AdminUsername;
            await _atmRepository.UpdateDataAsync(atmDetails);

            // Record admin transaction
            var transaction = new TransactionDetails
            {
                UserName = dto.AdminUsername,
                Type = TransactionType.Credit,
                Amount = dto.Amount,
                TimeStamp = DateTime.UtcNow,
                NewBalance = atmDetails.TotalBalance,
                IsAdminTransaction = true
            };

            await _transactionRepository.AddDataAsync(transaction);

            return true;
        }

    }
}
