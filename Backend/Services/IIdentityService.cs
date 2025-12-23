using Backend.DTOs;

using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IIdentityService
    {
        /// <summary>
        /// Authenticates a user or admin with username and PIN.
        /// </summary>
        Task<LoginResponseDto> LoginAsync(LoginDTO loginDto);

        /// <summary>
        /// Creates a new user account.
        /// </summary>
        Task<LoginResponseDto> SignupAsync(SignupDto signupDto);

        /// <summary>
        /// Changes the PIN for a user.
        /// </summary>
        Task<bool> ChangePinAsync(PinChangeDto pinChangeDto);

        /// <summary>
        /// Gets the account balance for a user.
        /// </summary>
        Task<BalanceDto> GetBalanceAsync(string username);

        /// <summary>
        /// Freezes a user account. Admin only operation.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Thrown when non-admin attempts this operation.</exception>
        Task<bool> FreezeAccountAsync(string username, string adminUsername);

        /// <summary>
        /// Unfreezes a user account. Admin only operation.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Thrown when non-admin attempts this operation.</exception>
        Task<bool> UnfreezeAccountAsync(UnfreezeUserDto dto);

        /// <summary>
        /// Gets a list of all frozen user accounts. Admin only operation.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Thrown when non-admin attempts this operation.</exception>
        Task<UserListDto> GetFrozenAccountsAsync(string adminUsername);
    }
}
