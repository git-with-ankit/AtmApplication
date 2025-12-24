using Backend.DTOs;

using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IIdentityService
    {
        Task<LoginResponseDto> LoginAsync(LoginDTO loginDto);
        Task<LoginResponseDto> SignupAsync(SignupDto signupDto);
        Task<bool> ChangePinAsync(PinChangeDto pinChangeDto);
        Task<bool> FreezeAccountAsync(string username);
        Task<bool> UnfreezeAccountAsync(UnfreezeUserDto dto);
        Task<UserListDto> GetFrozenAccountsAsync(string adminUsername);
        Task<bool> ChangeAdminAsync(ChangeAdminDto dto);
    }
}
