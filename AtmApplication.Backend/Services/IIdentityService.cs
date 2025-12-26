using AtmApplication.Backend.DTOs;

using System.Threading.Tasks;

namespace AtmApplication.Backend.Services
{
    public interface IIdentityService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<LoginResponseDto> SignupAsync(SignupDto signupDto);
        Task<bool> ChangePinAsync(PinChangeDto pinChangeDto);
        Task<bool> FreezeAccountAsync(string username);
        Task<bool> UnfreezeAccountAsync(UnfreezeUserDto dto);
        Task<FrozenAccountsListDto> GetFrozenAccountsAsync(string adminUsername);
        Task<bool> ChangeAdminAsync(ChangeAdminDto dto);
    }
}
