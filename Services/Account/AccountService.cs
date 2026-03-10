using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Repositories.AccountRepository;
using Ecommerce.Repositories.OTP;
using Ecommerce.Repositories.RankAccount;
using Ecommerce.Services.JWT;
using Ecommerce.Services.Mail;
using Ecommerce.Services.Vaild;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IVaildService vaildService;
        private readonly IRankAccountRepository rankAccountRepository;
        private readonly IAccountRepository accountRepository;
        private readonly IJwtService jwtService;
        private readonly IOTPRepository otpRepository;
        private readonly UserManager<Users> userManager;
        private readonly SignInManager<Users> signInManager;
        private readonly IMailService mailService;

        public AccountService(IVaildService vaildService, IRankAccountRepository rankAccountRepository, IAccountRepository accountRepository,
            UserManager<Users> userManager, SignInManager<Users> signInManager, IJwtService jwtService, IOTPRepository otpRepository, IMailService mailService)
        {
            this.vaildService = vaildService;
            this.rankAccountRepository = rankAccountRepository;
            this.accountRepository = accountRepository;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtService = jwtService;
            this.otpRepository = otpRepository;
            this.mailService = mailService;
        } 

        public async Task<(bool isSuccess, string Message)> RegisterAsync(RegisterViewModel model, IFormFile imageFile)
        {
            if (!vaildService.KiemTraNgaySinh(model.DateOfBirth, out string errorMessage))
                return (false, errorMessage);

            if (!vaildService.checkEmail(model.Email))
                return (false, "Email đã tồn tại.");

            if (vaildService.checkPhone(model.PhoneNumber))
                return (false, "Số điện thoại đã tồn tại vui lòng sử dụng Quên mật khẩu để lấy lại tài khoản");

            //Nếu không gán ảnh thì chọn ảnh mặc định lưu trong hệ thống
            var imagePath = string.IsNullOrEmpty(model.UrlImage)
            ? "/Images/UserAvatar/GuestUser.jpg"
            : model.UrlImage;

            //Lưu ảnh người dùng vào trong project
            if (imageFile != null)
                imagePath = await vaildService.SaveImage(imageFile, model.Name, model.PhoneNumber, "");

            //Kiểm tra rank và lấy rankId
            int rankId = await rankAccountRepository.SelectLastestRank();

            //Tạo tài khoản
            var result = await accountRepository.CreateAccountAsync(model, rankId, imagePath, "User");
            if (result.Succeeded == false)
            {
                return (false, "Đăng ký thất bại");
            }
            return (true, "Đăng ký thành công");
        }

        public async Task<LoginResultDTO> LoginAsync(LoginViewModel model)
        {
            //Kiểm tra Email đã đăng kí tài khoản chưa
            var user = await accountRepository.FindUserByEmail(model.Email);
            if (user == null)
            {
                return new LoginResultDTO { IsSuccess = false, Message = "Email chưa được đăng ký"};
            }

            //Thực hiện đăng nhập
            var result = await signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );


            if(result.Succeeded == false) return new LoginResultDTO { IsSuccess = false, Message = "Email hoặc mật khẩu không đúng" };

            //Tạo token
            string token = await jwtService.CreateToken(user);
            if (token == null) return new LoginResultDTO { IsSuccess = false, Message = "Token không được tạo" };

            //Lấy role và userName
            var (userName, role) = await accountRepository.GetUserandRole(model.Email);

            return new LoginResultDTO 
            { 
                IsSuccess = true, Token = token,
                UserName = userName,
                Role = role,
                Message = "Đăng nhập thành công" 
            };
        }

        public async Task<SendOTPResultDTO> SendOTPAsync(SendOTPViewModel model)
        {
            var user = await accountRepository.FindUserByEmail(model.Email);

            //Kiểm tra Email có tồn tại hay không
            if (user == null) return new SendOTPResultDTO { IsSuccess = false, Message = "Email chưa được đăng ký"};

            // Tạo mã OTP ngẫu nhiên 6 chữ số
            var OTPCode = new Random().Next(100000, 999999).ToString();

            //Kiểm tra User này đã gọi gửi OTP hay chưa nếu không thì tạo mới OTP ngược lại thì cập nhật OTPCode
            var result = await otpRepository.CreateOrUpdateOTP(user.Id, OTPCode);

            if(!result.IsSuccess) return new SendOTPResultDTO { IsSuccess = false, Message = result.Message };

            //Send OTP qua Email
            await mailService.SendMailAsync(model.Email,
                        $"OTP thay đổi mật khẩu: {OTPCode}", 
                        $"<p>Xin chào! Bạn đã yêu cầu thay đổi mật khẩu trên trang <strong>HUTECH RESTAURANT</strong>.</p>" +
                               $"<p>Mã OTP của bạn là: <strong>{OTPCode}</strong></p>" +
                               $"<p>Mã này có hiệu lực trong phiên yêu cầu của bạn.</p>"
            );

            return new SendOTPResultDTO { IsSuccess = true, Message = "Gửi OTP thành công", UserId = user.Id};
        }

        public async Task<VerifyOTPResult> VerifyOTPAsync(VerifyOTPViewModel model)
        {
            var otpItem = await otpRepository.FindByUserId(model.UserId);
            if (otpItem == null) return new VerifyOTPResult { IsSuccess = false, Message = "Tài khoản chưa gửi yêu cầu đổi mật khẩu" };

            var nowUtc = DateTime.Now;
            if (otpItem.TimeCreate.Date != nowUtc.Date)        // khác ngày
                return new VerifyOTPResult { IsSuccess = false, Message = "Mã OTP đã hết hạn"};

            if (otpItem.TimeCreate.AddMinutes(1) < nowUtc)     // quá 1 phút
                return new VerifyOTPResult { IsSuccess = false, Message = "Mã OTP đã hết hạn" };

            await otpRepository.UpdateOTPStatus("Success", model.UserId);
            return new VerifyOTPResult { IsSuccess = true, Message = "Xác thực OTP thành công", UserId=model.UserId};
        }

        public async Task<StatusDTO> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy người dùng." };
            }

            //Kiểm tra xem người dùng đã xác minh thành công OTP chưa
            var resultOTP = await otpRepository.CheckOTPStatus(model.UserId);
            if(resultOTP == false) return new StatusDTO {Message = "Chưa xác thực OTP", IsSuccess = false};

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return new StatusDTO { IsSuccess = false, Message = errors };
            }

            await otpRepository.DeleteOTP(model.UserId);
            await signInManager.SignOutAsync();
            return new StatusDTO { IsSuccess = true, Message = "Thay đổi mật khẩu thành công" };
        }

        public async Task<StatusDTO> Logout()
        {
            await signInManager.SignOutAsync();
            return new StatusDTO { IsSuccess = true, Message = "Đăng xuất thành công" };
        }
    }
}
