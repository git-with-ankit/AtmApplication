using AtmApplication.DataAccess;
using AtmApplication.DataAccess.Entities;
using AtmApplication.Backend.ApplicationConstants;
using AtmApplication.DataAccess.Interfaces;

namespace AtmApplication.Backend.Services
{
    public sealed class ValidationService : IValidationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;

        public ValidationService(
            IUserRepository userRepository,
            IAccountRepository accountRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

        public async Task ValidateAdminAsync(string username)
        {
            var user = await _userRepository.GetDataByUsernameAsync(username);
            if (user == null || !user.IsAdmin)
            {
                throw new UnauthorizedAccessException(ExceptionMessages.UnauthorizedAccess);
            }
        }

        public async Task ValidateAccountExistsAsync(string username)
        {
            var account = await _accountRepository.GetDataByUsernameAsync(username);
            if (account == null)
            {
                throw new InvalidOperationException(
                    string.Format(ExceptionMessages.AccountNotFound, username));
            }
        }

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
