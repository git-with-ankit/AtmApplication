using Backend.DTOs;
using Backend.ApplicationConstants;
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
        private readonly IRepository<AtmDetails> _atmRepository;

        public IdentityService(
            IValidationService validationService,
            IRepository<UserDetails> userRepository,
            IRepository<AccountDetails> accountRepository,
            IRepository<AtmDetails> atmRepository)
        {
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _atmRepository = atmRepository ?? throw new ArgumentNullException(nameof(atmRepository));
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDTO loginDto)
        {
            var user = await _userRepository.GetDataByUsernameAsync(loginDto.Username);
            
            if (user is null)
            {
                return new LoginResponseDto
                {
                    Username = string.Empty,
                    IsAdmin = false,
                    IsLoginSuccessful = false,
                    IsFrozen = false
                };
            }

            if (user.IsFreezed)
            {
                throw new AccountFrozenException();
            }
            if (user.Pin != loginDto.Pin)
            {
                user.FailedLoginAttempts++;
                await _userRepository.UpdateDataAsync(user);

                if (user.FailedLoginAttempts >= Constants.MaxPinAttempts)
                {
                    user.IsFreezed = true;
                    await _userRepository.UpdateDataAsync(user);
                    throw new ExceededPinAttemptsException();
                }

                return new LoginResponseDto
                {
                    Username = string.Empty,
                    IsAdmin = false,
                    IsLoginSuccessful = false,
                    IsFrozen = false,
                    Message = $"Invalid PIN. {Constants.MaxPinAttempts - user.FailedLoginAttempts} attempts remaining."
                };
            }

            user.FailedLoginAttempts = 0;
            await _userRepository.UpdateDataAsync(user);

            return new LoginResponseDto
            {
                Username = user.Username,
                IsAdmin = user.IsAdmin,
                IsLoginSuccessful = true,
                IsFrozen = false
            };
        } 
         
        public async Task<LoginResponseDto> SignupAsync(SignupDto signupDto)
        {
            var existingUser = await _userRepository.GetDataByUsernameAsync(signupDto.Username);
            if (existingUser != null)
            {
                throw new UsernameTakenException();
            }

            var newUser = new UserDetails
            {
                Username = signupDto.Username,
                Pin = signupDto.Pin,
                IsAdmin = false,
                IsFreezed = false
            };

            await _userRepository.AddDataAsync(newUser);

            var newAccount = new AccountDetails
            {
                Username = signupDto.Username,
                Balance = 0.0
            };

            await _accountRepository.AddDataAsync(newAccount);

            return new LoginResponseDto
            {
                Username = newUser.Username,
                IsAdmin = newUser.IsAdmin,
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

            if (user.Pin != pinChangeDto.CurrentPin)
            {
                return false;
            }

            user.Pin = pinChangeDto.NewPin;
            await _userRepository.UpdateDataAsync(user);

            return true;
        }


        public async Task<bool> FreezeAccountAsync(string username)
        {
            var user = await _userRepository.GetDataByUsernameAsync(username);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            // CRITICAL: Never freeze admin accounts - if admin is frozen, application becomes unusable
            if (user.IsAdmin)
            {
                throw new AdminFreezeException();
            }

            user.IsFreezed = true;
            await _userRepository.UpdateDataAsync(user);

            return true;
        }

        public async Task<bool> UnfreezeAccountAsync(UnfreezeUserDto dto)
        {
            await _validationService.ValidateAdminAsync(dto.AdminUsername);

            var user = await _userRepository.GetDataByUsernameAsync(dto.Username);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            // Check if account is actually frozen
            if (!user.IsFreezed)
            {
                throw new InvalidOperationException(ExceptionMessages.AccountNotFrozen);
            }

            user.IsFreezed = false;
            user.FailedLoginAttempts = 0; // Reset failed attempts when unfreezing
            await _userRepository.UpdateDataAsync(user);

            return true;
        }

        public async Task<UserListDto> GetFrozenAccountsAsync(string adminUsername)
        {
            await _validationService.ValidateAdminAsync(adminUsername);

            var allUsers = await _userRepository.GetAllDataAsync();
            var frozenUsers = allUsers.Where(u => u.IsFreezed).ToList();
            if (frozenUsers.Any())
            {
                var firstFrozen = frozenUsers.First();//return the list of frozen users
                var account = await _accountRepository.GetDataByUsernameAsync(firstFrozen.Username);

                return new UserListDto
                {
                    Username = firstFrozen.Username,
                    IsAdmin = firstFrozen.IsAdmin,
                    IsLoginSuccessful = !firstFrozen.IsFreezed,
                    IsFrozen = firstFrozen.IsFreezed,
                    Balance = account != null ? account.Balance : 0
                };
            }

            return new UserListDto
            {
                Username = string.Empty,
                IsAdmin = false,
                IsLoginSuccessful = false,
                IsFrozen = false,
                Balance = 0
            };
        }

        public async Task<bool> ChangeAdminAsync(ChangeAdminDto dto)
        {
            await _validationService.ValidateAdminAsync(dto.CurrentAdminUsername);

            var currentAdmin = await _userRepository.GetDataByUsernameAsync(dto.CurrentAdminUsername);
            if (currentAdmin == null || !currentAdmin.IsAdmin)
            {
                return false;
            }

            // Check if new admin username is already taken
            var existingUser = await _userRepository.GetDataByUsernameAsync(dto.NewAdminUsername);
            if (existingUser != null)
            {
                throw new UsernameTakenException();
            }

            // Delete old admin user record (admins don't have accounts, so no need to delete from AccountRepository)
            await _userRepository.DeleteDataByUsernameAsync(dto.CurrentAdminUsername);

            // Create NEW admin user (not promoting existing user)
            var newAdmin = new UserDetails
            {
                Username = dto.NewAdminUsername,
                Pin = dto.NewAdminPin,
                IsAdmin = true,
                IsFreezed = false,
                FailedLoginAttempts = 0
            };
            await _userRepository.AddDataAsync(newAdmin);

            // Update ATM admin username
            var atmDetails = (await _atmRepository.GetAllDataAsync()).FirstOrDefault();
            if (atmDetails != null)
            {
                atmDetails.AdminUsername = dto.NewAdminUsername;
                await _atmRepository.UpdateDataAsync(atmDetails);
            }

            return true;
        }
    }
}
