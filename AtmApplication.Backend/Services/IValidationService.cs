using AtmApplication.Backend.DTOs;
using AtmApplication.DataAccess;
using AtmApplication.DataAccess.Entities;
using AtmApplication.Backend.Exceptions;

namespace AtmApplication.Backend.Services
{
    public interface IValidationService
    {
        Task ValidateAdminAsync(string username);
        Task ValidateAccountExistsAsync(string username);
        Task ValidateUserExistsAsync(string username);
        bool ValidateUsernameFormat(string username);
        Task ValidateAccountNotFrozenAsync(string username);
        Task<PinVerificationResponseDto> VerifyPinWithAttemptsAsync(string username, int pin);
    }
}
