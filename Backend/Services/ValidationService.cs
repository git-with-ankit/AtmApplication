using DataAccess;
using DataAccess.Entities;
using Backend.ApplicationConstants;

namespace Backend.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IRepository<UserDetails> _userRepository;
        private readonly IRepository<AccountDetails> _accountRepository;

        public ValidationService(
            IRepository<UserDetails> userRepository,
            IRepository<AccountDetails> accountRepository)
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
