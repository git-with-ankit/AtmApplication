using Backend.DTOs;

using Backend.Exceptions;
using DataAccess;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IValidationService _validationService;
        private readonly IRepository<AccountDetails> _accountRepository;
        private readonly IRepository<AtmDetails> _atmRepository;
        private readonly IRepository<TransactionDetails> _transactionRepository;

        public TransactionService(
            IValidationService validationService,
            IRepository<AccountDetails> accountRepository,
            IRepository<AtmDetails> atmRepository,
            IRepository<TransactionDetails> transactionRepository)
        {
            _validationService = validationService;
            _accountRepository = accountRepository;
            _atmRepository = atmRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<TransactionResponseDto> DepositAsync(TransactionDto transactionDto)
        {
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
                Amount = (double)transactionDto.Amount,
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

        public async Task<TransactionResponseDto> WithdrawAsync(TransactionDto transactionDto)
        {
            var account = await _accountRepository.GetDataByUsernameAsync(transactionDto.Username);
            if (account == null)
            {
                throw new InvalidOperationException(ExceptionMessages.AccountNotFound);
            }

            // Check user balance
            if (account.Balance < (double)transactionDto.Amount)
            {
                throw new InsufficientFundsException();
            }

            // Check ATM balance
            var atmDetails = (await _atmRepository.GetAllDataAsync()).FirstOrDefault();
            if (atmDetails == null || atmDetails.TotalBalance < transactionDto.Amount)
            {
                throw new InsufficientFundsException();
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
            var allTransactions = await _transactionRepository.GetAllDataAsync();
            var userTransactions = allTransactions
                .Where(t => t.UserName.Equals(username))
                .OrderByDescending(t => t.TimeStamp)
                .Take(count)
                .Select(t => new TransactionHistoryItemDto
                {
                    Type = t.Type,
                    Amount = t.Amount,
                    NewBalance = t.NewBalance,
                    Timestamp = t.TimeStamp
                })
                .ToList();

            return new TransactionsHistoryDto
            {
                Transactions = userTransactions
            };
        }

        public async Task<BalanceDto> GetBalanceAsync(string username)
        {
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
                // Create new ATM record if it doesn't exist
                atmDetails = new AtmDetails
                {
                    AdminUsername = dto.AdminUsername,
                    TotalBalance = dto.Amount
                };
                await _atmRepository.AddDataAsync(atmDetails);
            }
            else
            {
                // Update balance and current admin
                atmDetails.TotalBalance += dto.Amount;
                atmDetails.AdminUsername = dto.AdminUsername;
                await _atmRepository.UpdateDataAsync(atmDetails);
            }

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
