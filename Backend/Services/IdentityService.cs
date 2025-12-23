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
    public class IdentityService : IIdentityService
    {
        private readonly IValidationService _validationService;
        private readonly IRepository<UserDetails> _userRepository;
        private readonly IRepository<AccountDetails> _accountRepository;
        private readonly Dictionary<string, int> _loginAttempts = new();
        private const int MaxLoginAttempts = 3;

        public IdentityService(
            IValidationService validationService,
            IRepository<UserDetails> userRepository,
            IRepository<AccountDetails> accountRepository)
        {
            _validationService = validationService;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDTO loginDto)
        {
            var user = await _userRepository.GetDataByUsernameAsync(loginDto.Username);
            
            if (user == null)
            {
                return new LoginResponseDto
                {
                    Username = string.Empty,
                    Role = UserRole.User,
                    IsLoginSuccessful = false,
                    IsFrozen = false
                };
            }

            // Check if account is frozen
            if (user.IsFreezed)
            {
                throw new AccountFrozenException();
            }

            // Check if PIN matches
            if (user.Pin != loginDto.Pin)
            {
                // Track failed login attempts
                _loginAttempts.TryGetValue(loginDto.Username, out int attempts);
                attempts++;
                _loginAttempts[loginDto.Username] = attempts;

                if (attempts >= MaxLoginAttempts)
                {
                    // Freeze account after 3 failed attempts
                    user.IsFreezed = true;
                    await _userRepository.UpdateDataAsync(user);
                    throw new ExceededPinAttemptsException();
                }

                return new LoginResponseDto
                {
                    Username = string.Empty,
                    Role = UserRole.User,
                    IsLoginSuccessful = false,
                    IsFrozen = false,
                    Message = $"Invalid PIN. {MaxLoginAttempts - attempts} attempts remaining."
                };
            }

            // Successful login - clear attempts
            _loginAttempts.Remove(loginDto.Username);

            return new LoginResponseDto
            {
                Username = user.Username,
                Role = user.Role,
                IsLoginSuccessful = true,
                IsFrozen = false
            };
        }

        public async Task<LoginResponseDto> SignupAsync(SignupDto signupDto)
        {
            // Check if username already exists
            var existingUser = await _userRepository.GetDataByUsernameAsync(signupDto.Username);
            if (existingUser != null)
            {
                throw new UsernameTakenException();
            }

            // Create new user
            var newUser = new UserDetails
            {
                Username = signupDto.Username,
                Pin = signupDto.Pin,
                Role = signupDto.Role,
                IsFreezed = false
            };

            await _userRepository.AddDataAsync(newUser);

            // Create account with zero balance
            var newAccount = new AccountDetails
            {
                Username = signupDto.Username,
                Balance = 0.0
            };

            await _accountRepository.AddDataAsync(newAccount);

            return new LoginResponseDto
            {
                Username = newUser.Username,
                Role = newUser.Role,
                IsLoginSuccessful = true,
                IsFrozen = false
            };
        }

        public async Task<bool> ChangePinAsync(PinChangeDto pinChangeDto)
        {
            var user = await _userRepository.GetDataByUsernameAsync(pinChangeDto.Username);
            if (user == null)
            {
                return false;
            }

            // Verify current PIN
            if (user.Pin != pinChangeDto.CurrentPin)
            {
                return false;
            }

            // Update to new PIN
            user.Pin = pinChangeDto.NewPin;
            await _userRepository.UpdateDataAsync(user);

            return true;
        }


        public async Task<bool> FreezeAccountAsync(string username, string adminUsername)
        {
            // Validate admin
            await _validationService.ValidateAdminAsync(adminUsername);

            var user = await _userRepository.GetDataByUsernameAsync(username);
            if (user == null)
            {
                return false;
            }

            user.IsFreezed = true;
            await _userRepository.UpdateDataAsync(user);

            return true;
        }

        public async Task<bool> UnfreezeAccountAsync(UnfreezeUserDto dto)
        {
            // Validate admin
            await _validationService.ValidateAdminAsync(dto.AdminUsername);

            var user = await _userRepository.GetDataByUsernameAsync(dto.Username);
            if (user == null)
            {
                return false;
            }

            user.IsFreezed = false;
            await _userRepository.UpdateDataAsync(user);

            // Clear login attempts when unfreezing
            _loginAttempts.Remove(dto.Username);

            return true;
        }

        public async Task<UserListDto> GetFrozenAccountsAsync(string adminUsername)
        {
            // Validate admin
            await _validationService.ValidateAdminAsync(adminUsername);

            var allUsers = await _userRepository.GetAllDataAsync();
            var frozenUsers = allUsers.Where(u => u.IsFreezed).ToList();

            // For simplicity, returning the first frozen user
            // In a real scenario, you might want to return a list
            if (frozenUsers.Any())
            {
                var firstFrozen = frozenUsers.First();
                var account = await _accountRepository.GetDataByUsernameAsync(firstFrozen.Username);

                return new UserListDto
                {
                    Username = firstFrozen.Username,
                    Role = firstFrozen.Role,
                    IsLoginSuccessful = !firstFrozen.IsFreezed,
                    IsFrozen = firstFrozen.IsFreezed,
                    Balance = account != null ? account.Balance : 0
                };
            }

            return new UserListDto
            {
                Username = string.Empty,
                Role = UserRole.User,
                IsLoginSuccessful = false,
                IsFrozen = false,
                Balance = 0
            };
        }
    }
}
