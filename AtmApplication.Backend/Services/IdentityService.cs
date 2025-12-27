using AtmApplication.Backend.DTOs;
using AtmApplication.Backend.ApplicationConstants;
using AtmApplication.Backend.Exceptions;
using AtmApplication.DataAccess.Entities;
using AtmApplication.DataAccess.Interfaces;    


namespace AtmApplication.Backend.Services
{
    public sealed class IdentityService : IIdentityService
    {
        private readonly IValidationService _validationService;
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAtmRepository _atmRepository;

        public IdentityService(
            IValidationService validationService,
            IUserRepository userRepository,
            IAccountRepository accountRepository,
            IAtmRepository atmRepository)
        {
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _atmRepository = atmRepository ?? throw new ArgumentNullException(nameof(atmRepository));
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetDataByUsernameAsync(loginDto.Username);
            
            if (user is null)
            {
                throw new UserNotFoundException();
            }
            await _validationService.ValidateAccountNotFrozenAsync(loginDto.Username);
            var verificationResult = await _validationService.VerifyPinWithAttemptsAsync(loginDto.Username, loginDto.Pin);

            if (!verificationResult.IsVerified)
            {
                return new LoginResponseDto
                {
                    Username = string.Empty,
                    IsAdmin = false,
                    IsLoginSuccessful = false,
                    IsFrozen = false,
                    Message = verificationResult.Message
                };
            }

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
            await _validationService.ValidateUsernameAvailableAsync(signupDto.Username);

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
            var user = await _validationService.ValidateUserExistsAsync(pinChangeDto.Username);

            if (user.Pin != pinChangeDto.CurrentPin)
            {
                throw new InvalidCredentialsException();
            }

            user.Pin = pinChangeDto.NewPin;
            await _userRepository.UpdateDataAsync(user);

            return true;
        }


        public async Task<bool> FreezeAccountAsync(string username)
        {
            var user = await _validationService.ValidateUserExistsAsync(username);

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

            var user = await _validationService.ValidateUserExistsAsync(dto.Username);

            if (!user.IsFreezed)
            {
                throw new InvalidOperationException(ExceptionMessages.AccountNotFrozen);
            }

            user.IsFreezed = false;
            user.FailedLoginAttempts = 0;
            await _userRepository.UpdateDataAsync(user);

            return true;
        }

        public async Task<FrozenAccountsListDto> GetFrozenAccountsAsync(string adminUsername)
        {
            await _validationService.ValidateAdminAsync(adminUsername);

            var allUsers = await _userRepository.GetAllDataAsync();
            var frozenUsers = allUsers.Where(u => u.IsFreezed).ToList();
            
            var frozenAccountsList = new List<FrozenAccountDto>();

            foreach (var frozenUser in frozenUsers)
            {
                var account = await _accountRepository.GetDataByUsernameAsync(frozenUser.Username);
                frozenAccountsList.Add(new FrozenAccountDto
                {
                    Username = frozenUser.Username,
                    Balance = account != null ? account.Balance : 0
                });
            }

            return new FrozenAccountsListDto
            {
                FrozenAccounts = frozenAccountsList
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
            await _validationService.ValidateUsernameAvailableAsync(dto.NewAdminUsername);
            await _userRepository.DeleteDataByUsernameAsync(dto.CurrentAdminUsername);
            var newAdmin = new UserDetails
            {
                Username = dto.NewAdminUsername,
                Pin = dto.NewAdminPin,
                IsAdmin = true,
                IsFreezed = false,
                FailedLoginAttempts = 0
            };
            await _userRepository.AddDataAsync(newAdmin);

            var atmDetails = (await _atmRepository.GetAllDataAsync()).FirstOrDefault();
            if (atmDetails == null)
            {
                throw new InvalidOperationException(ExceptionMessages.AtmNotFound);
            }
            atmDetails.AdminUsername = dto.NewAdminUsername;
            await _atmRepository.UpdateDataAsync(atmDetails);

            return true;
        }
    }
}


