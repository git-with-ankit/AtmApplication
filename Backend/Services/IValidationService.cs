using DataAccess;
using DataAccess.Entities;
using Backend.Exceptions;

namespace Backend.Services
{
    public interface IValidationService
    {
        Task ValidateAdminAsync(string username);
        Task ValidateAccountExistsAsync(string username);
        Task ValidateUserExistsAsync(string username);
    }
}
