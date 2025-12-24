using AtmApplication.Backend.DTOs;

using System.Threading.Tasks;

namespace AtmApplication.Backend.Services
{
    public interface ITransactionService
    {
        Task<TransactionResponseDto> DepositAsync(TransactionDto transactionDto);
        Task<TransactionResponseDto> WithdrawAsync(TransactionDto transactionDto);
        Task<BalanceDto> GetBalanceAsync(string username);
        Task<TransactionsHistoryDto> GetTransactionHistoryAsync(string username, int count);
        Task<AtmBalanceDto> GetAtmBalanceAsync(string adminUsername);
        Task<bool> DepositToAtmAsync(DepositCashDto dto);
    }
}
