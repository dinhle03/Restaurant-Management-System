using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.ViewModels;
using System.Runtime.CompilerServices;

namespace Ecommerce.Services.Account
{
    public interface IAccountService
    {
        Task<(bool isSuccess, string Message)> RegisterAsync(RegisterViewModel model, IFormFile imageFile);
        Task<LoginResultDTO> LoginAsync(LoginViewModel model);
        Task<SendOTPResultDTO> SendOTPAsync(SendOTPViewModel model);
        Task<VerifyOTPResult> VerifyOTPAsync(VerifyOTPViewModel model);
        Task<StatusDTO> ChangePasswordAsync(ChangePasswordViewModel model);
        Task<StatusDTO> Logout();
    }
}
