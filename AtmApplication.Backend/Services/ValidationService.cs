using AtmApplication.Backend.ApplicationConstants;
using AtmApplication.Backend.DTOs;
using AtmApplication.Backend.Exceptions;
using AtmApplication.DataAccess;
using AtmApplication.DataAccess.Entities;
using AtmApplication.DataAccess.Interfaces;
using System.Text.RegularExpressions;

namespace AtmApplication.Backend.Services
{
    public sealed class ValidationService : IValidationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private static readonly Regex UsernameRegex = new(@"^[a-zA-Z0-9_]{" + Constants.MinUsernameLength + "," + Constants.MaxUsernameLength + "}$", RegexOptions.Compiled);
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

        public bool ValidateUsernameFormat(string username)
        {
            return !string.IsNullOrWhiteSpace(username) && UsernameRegex.IsMatch(username);
        }

        public async Task ValidateAccountNotFrozenAsync(string username)
        {
            var user = await _userRepository.GetDataByUsernameAsync(username);
            if (user == null)
            {
                throw new InvalidOperationException(
                    string.Format(ExceptionMessages.UserNotFound, username));
            }

            if (user.IsFreezed)
            {
                throw new AccountFrozenException();
            }
        }

        public async Task<PinVerificationResponseDto> VerifyPinWithAttemptsAsync(string username, int pin)
        {
            var user = await _userRepository.GetDataByUsernameAsync(username);
            if (user == null)
            {
                throw new InvalidOperationException(
                    string.Format(ExceptionMessages.UserNotFound, username));
            }

            // Verify PIN
            if (user.Pin != pin)
            {
                user.FailedLoginAttempts++;
                if (user.FailedLoginAttempts >= Constants.MaxPinAttempts)
                {
                    user.IsFreezed = true;
                    await _userRepository.UpdateDataAsync(user);
                    throw new ExceededPinAttemptsException();
                }
                else
                {
                    await _userRepository.UpdateDataAsync(user);
                }

                return new PinVerificationResponseDto
                {
                    IsVerified = false,
                    RemainingAttempts = Constants.MaxPinAttempts - user.FailedLoginAttempts,
                    IsAccountFrozen = false,
                    Message = $"Invalid PIN. {Constants.MaxPinAttempts - user.FailedLoginAttempts} attempts remaining."
                };
            }

            // PIN is correct - reset failed attempts
            user.FailedLoginAttempts = 0;
            await _userRepository.UpdateDataAsync(user);

            return new PinVerificationResponseDto
            {
                IsVerified = true,
                RemainingAttempts = Constants.MaxPinAttempts,
                IsAccountFrozen = false,
                Message = "PIN verified successfully."
            };
        }
    }
}
