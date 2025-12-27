using AtmApplication.Backend.ApplicationConstants;
using AtmApplication.Backend.DTOs;
using AtmApplication.DataAccess.Interfaces;
using AtmApplication.Backend.Exceptions;
using AtmApplication.DataAccess.Entities;


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

        private async Task<AtmDetails> GetAtmDetailsAsync()
        {
            return (await _atmRepository.GetAllDataAsync()).FirstOrDefault()
                ?? throw new InvalidOperationException(ExceptionMessages.AtmNotFound);
        }

        private async Task RecordTransactionAsync(
            string username,
            TransactionType type,
            double amount,
            double newBalance,
            bool isAdminTransaction)
        {
            var transaction = new TransactionDetails
            {
                UserName = username,
                Type = type,
                Amount = amount,
                TimeStamp = DateTime.UtcNow,
                NewBalance = newBalance,
                IsAdminTransaction = isAdminTransaction
            };
            await _transactionRepository.AddDataAsync(transaction);
        }

        private TransactionResponseDto CreateTransactionResponse(
            TransactionDto transactionDto,
            double newBalance,
            DateTime timestamp)
        {
            return new TransactionResponseDto
            {
                Username = transactionDto.Username,
                Type = transactionDto.Type,
                Amount = transactionDto.Amount,
                NewBalance = newBalance,
                Timestamp = timestamp
            };
        }

        public async Task<TransactionResponseDto> DepositAsync(TransactionDto transactionDto, int pin)
        {
            await _validationService.ValidateUserAndPinAsync(transactionDto.Username, pin);

            var account = await _validationService.ValidateAccountExistsAsync(transactionDto.Username);
            account.Balance += transactionDto.Amount;
            await _accountRepository.UpdateDataAsync(account);

            var atmDetails = await GetAtmDetailsAsync();
            atmDetails.TotalBalance += transactionDto.Amount;
            await _atmRepository.UpdateDataAsync(atmDetails);

            var timestamp = DateTime.UtcNow;
            await RecordTransactionAsync(
                transactionDto.Username,
                transactionDto.Type,
                transactionDto.Amount,
                account.Balance,
                isAdminTransaction: false);

            return CreateTransactionResponse(transactionDto, account.Balance, timestamp);
        }

        public async Task<TransactionResponseDto> WithdrawAsync(TransactionDto transactionDto, int pin)
        {
            await _validationService.ValidateUserAndPinAsync(transactionDto.Username, pin);

            var account = await _validationService.ValidateAccountExistsAsync(transactionDto.Username);

            if (account.Balance < transactionDto.Amount)
            {
                throw new InsufficientFundsException();
            }
            var atmDetails = await GetAtmDetailsAsync();
            if (atmDetails.TotalBalance < transactionDto.Amount)
            {
                throw new InvalidOperationException();
            }

            account.Balance -= transactionDto.Amount;
            await _accountRepository.UpdateDataAsync(account);

            atmDetails.TotalBalance -= transactionDto.Amount;
            await _atmRepository.UpdateDataAsync(atmDetails);

            var timestamp = DateTime.UtcNow;
            await RecordTransactionAsync(
                transactionDto.Username,
                transactionDto.Type,
                transactionDto.Amount,
                account.Balance,
                isAdminTransaction: false);

            return CreateTransactionResponse(transactionDto, account.Balance, timestamp);
        }

        public async Task<TransactionsHistoryDto> GetTransactionHistoryAsync(string username, int count)
        {
            var atmDetails = (await _atmRepository.GetAllDataAsync()).FirstOrDefault();
            bool isAdmin = atmDetails != null && atmDetails.AdminUsername.Equals(username);

            var allTransactions = await _transactionRepository.GetAllDataAsync();
            
            IEnumerable<TransactionDetails> filteredTransactions;
            
            if (isAdmin)
            {
                filteredTransactions = allTransactions
                    .OrderByDescending(t => t.TimeStamp)
                    .Take(count);
            }
            else
            {
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
            await _validationService.ValidateUserAndPinAsync(username, pin);

            var account = await _validationService.ValidateAccountExistsAsync(username);

            return new BalanceDto
            {
                Username = username,
                Balance = account.Balance
            };
        }

        public async Task<AtmBalanceDto> GetAtmBalanceAsync(string adminUsername)
        {
            await _validationService.ValidateAdminAsync(adminUsername);

            var atmDetails = await GetAtmDetailsAsync();

            return new AtmBalanceDto
            {
                TotalBalance = atmDetails.TotalBalance
            };
        }

        public async Task<bool> DepositToAtmAsync(DepositCashDto dto)
        {
            await _validationService.ValidateAdminAsync(dto.AdminUsername);

            var atmDetails = await GetAtmDetailsAsync();

            atmDetails.TotalBalance += dto.Amount;
            atmDetails.AdminUsername = dto.AdminUsername;
            await _atmRepository.UpdateDataAsync(atmDetails);

            await RecordTransactionAsync(
                dto.AdminUsername,
                TransactionType.Credit,
                dto.Amount,
                atmDetails.TotalBalance,
                isAdminTransaction: true);

            return true;
        }

    }
}
