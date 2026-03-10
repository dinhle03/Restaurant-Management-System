using Ecommerce.DTO;
using Ecommerce.Models;

namespace Ecommerce.Repositories.OTP
{
    public interface IOTPRepository
    {
        Task<StatusDTO> CreateOrUpdateOTP(string userId, string OTPCode);
        Task<Models.OTP> FindByUserId(string userId);
        Task DeleteOTP(string userId);
        Task UpdateOTPStatus(string status, string userId);
        Task<bool> CheckOTPStatus(string userId);
    }
}
