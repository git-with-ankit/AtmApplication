using Backend.DTOs;

using System.Threading.Tasks;

namespace Backend.Services
{
    public interface ITransactionService
    {
        /// <summary>
        /// Deposits money into a user's account.
        /// </summary>
        Task<TransactionResponseDto> DepositAsync(TransactionDto transactionDto);

        /// <summary>
        /// Withdraws money from a user's account.
        /// </summary>
        Task<TransactionResponseDto> WithdrawAsync(TransactionDto transactionDto);

        /// <summary>
        /// Gets the account balance for a user.
        /// </summary>
        Task<BalanceDto> GetBalanceAsync(string username);

        /// <summary>
        /// Gets transaction history for a user.
        /// </summary>
        Task<TransactionsHistoryDto> GetTransactionHistoryAsync(string username, int count);

        /// <summary>
        /// Gets the current ATM balance. Admin only operation.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Thrown when non-admin attempts this operation.</exception>
        Task<AtmBalanceDto> GetAtmBalanceAsync(string adminUsername);

        /// <summary>
        /// Admin deposits cash into the ATM. Admin only operation.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Thrown when non-admin attempts this operation.</exception>
        Task<bool> DepositToAtmAsync(DepositCashDto dto);
    }
}
