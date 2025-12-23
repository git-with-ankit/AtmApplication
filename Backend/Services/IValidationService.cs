using DataAccess;
using DataAccess.Entities;
using Backend.Exceptions;

namespace Backend.Services
{
    /// <summary>
    /// Service for centralized validation logic across the application.
    /// Handles admin authorization, account existence, and user existence checks.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Validates that the specified user is an admin.
        /// </summary>
        /// <param name="username">Username to validate</param>
        /// <exception cref="UnauthorizedAccessException">Thrown when user is not an admin</exception>
        Task ValidateAdminAsync(string username);

        /// <summary>
        /// Validates that an account exists for the specified username.
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <exception cref="InvalidOperationException">Thrown when account does not exist</exception>
        Task ValidateAccountExistsAsync(string username);

        /// <summary>
        /// Validates that a user exists with the specified username.
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <exception cref="InvalidOperationException">Thrown when user does not exist</exception>
        Task ValidateUserExistsAsync(string username);
    }
}
