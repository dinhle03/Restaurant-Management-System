using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Ecommerce.Repositories.OTP
{
    public class OTPRepository : IOTPRepository
    {
        private readonly AppDbContext db;

        public OTPRepository(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<StatusDTO> CreateOrUpdateOTP(string userId, string OTPCode)
        {
            var otpItem = db.OTPs.FirstOrDefault(o => o.UserId == userId);
            if (otpItem == null)
            {
                //Create
                var otpEntity = new Models.OTP
                {
                    UserId = userId,
                    OTPCode = OTPCode,
                    TimeCreate = DateTime.Now,
                    Status = "Chưa xác thực",
                };
                db.OTPs.Add(otpEntity);
            }
            else
            {
                //Update
                otpItem.OTPCode = OTPCode;
                otpItem.TimeCreate = DateTime.Now;
                db.OTPs.Update(otpItem);
            }

            await db.SaveChangesAsync();
            return new StatusDTO {IsSuccess = true, Message = "Tạo OTP thành công"};
        }

        public async Task<Models.OTP> FindByUserId(string userId)
        {
            var otpItem = await db.OTPs.FirstOrDefaultAsync(o => o.UserId == userId);
            return otpItem;
        }

        public async Task DeleteOTP(string userId)
        {
            var otpITem = await db.OTPs.FirstOrDefaultAsync(otp => otp.UserId == userId);
            db.OTPs.Remove(otpITem);
            await db.SaveChangesAsync();
        } 

        public async Task UpdateOTPStatus(string status, string userId)
        {
            var otpItem = db.OTPs.FirstOrDefault(otp => otp.UserId == userId);
            if (otpItem != null)
            {
                otpItem.Status = status;
                db.Update(otpItem);
                await db.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckOTPStatus(string userId)
        {
            var otpItem = await db.OTPs.FirstOrDefaultAsync(otp => otp.UserId == userId);
            if (otpItem != null)
            {
                if (otpItem.Status == "Success") return true;
            }
                
            return false;
        }
    }
}
