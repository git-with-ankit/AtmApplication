using DataAccess;
using DataAccess.Entities;
using Backend.Exceptions;

namespace Backend.Services
{
    /// <summary>
    /// Implementation of validation service for centralized validation logic.
    /// </summary>
    public class ValidationService : IValidationService
    {
        private readonly IRepository<UserDetails> _userRepository;
        private readonly IRepository<AccountDetails> _accountRepository;

        public ValidationService(
            IRepository<UserDetails> userRepository,
            IRepository<AccountDetails> accountRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }

        /// <summary>
        /// Validates that the specified user is an admin.
        /// </summary>
        public async Task ValidateAdminAsync(string username)
        {
            var user = await _userRepository.GetDataByUsernameAsync(username);
            if (user == null || user.Role != UserRole.Admin)
            {
                throw new UnauthorizedAccessException(ExceptionMessages.UnauthorizedAccess);
            }
        }

        /// <summary>
        /// Validates that an account exists for the specified username.
        /// </summary>
        public async Task ValidateAccountExistsAsync(string username)
        {
            var account = await _accountRepository.GetDataByUsernameAsync(username);
            if (account == null)
            {
                throw new InvalidOperationException(
                    string.Format(ExceptionMessages.AccountNotFound, username));
            }
        }

        /// <summary>
        /// Validates that a user exists with the specified username.
        /// </summary>
        public async Task ValidateUserExistsAsync(string username)
        {
            var user = await _userRepository.GetDataByUsernameAsync(username);
            if (user == null)
            {
                throw new InvalidOperationException(
                    string.Format(ExceptionMessages.UserNotFound, username));
            }
        }
    }
}
