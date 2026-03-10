using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Repositories.AccountRepository;
using Ecommerce.Repositories.EmployeeRepository;
using Ecommerce.Services.Account;
using Ecommerce.Services.Vaild;
using Ecommerce.ViewModels;
using MailKit;
using System.Runtime.ConstrainedExecution;

namespace Ecommerce.Services.EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IVaildService vaildService;

        public EmployeeService(IEmployeeRepository employeeRepository, IVaildService vaildService)
        {
            this.employeeRepository = employeeRepository;
            this.vaildService = vaildService;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllAsync(string? search)
        {
            var users = await employeeRepository.GetAllAsync(search);
            return users.Select(u => new EmployeeDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                UrlImage = u.UrlImage,
            });
        }

        public async Task<EmployeeDTO?> GetByIdAsync(string id)
        {
            var user = await employeeRepository.GetByIdAsync(id);
            if (user == null) return null;
            var roles = await employeeRepository.GetRolesAsync(user);

            return new EmployeeDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                UrlImage = user.UrlImage,
            };
        }

        public async Task<StatusDTO> CreateAsync(RegisterViewModel model, IFormFile? imageFile)
        {
            if (!vaildService.KiemTraNgaySinh(model.DateOfBirth, out string errorMessage))
                return new StatusDTO { IsSuccess = false, Message = errorMessage };

            if (!vaildService.checkEmail(model.Email))
                return new StatusDTO { IsSuccess = false, Message = "Email đã tồn tại." };

            //Nếu không gán ảnh thì chọn ảnh mặc định lưu trong hệ thống
            var imagePath = string.IsNullOrEmpty(model.UrlImage)
            ? "/Images/UserAvatar/GuestUser.jpg"
            : model.UrlImage;

            //Lưu ảnh người dùng vào trong project
            if (imageFile != null)
                imagePath = await vaildService.SaveImage(imageFile, model.Name, model.PhoneNumber, "");


            var result = await employeeRepository.CreateAsync(model, imagePath, "Staff");
            if (!result.Succeeded) return new StatusDTO { IsSuccess = false, Message = string.Join(";", result.Errors.Select(e => e.Description)) };

            return new StatusDTO { IsSuccess = true, Message = "Tạo tài khoản nhân viên thành công" };
        }

        public async Task<StatusDTO> UpdateAsync(string id, Users model, IFormFile? imageFile)
        {
            var user = await employeeRepository.GetByIdAsync(id);
            if (user == null) return new StatusDTO {IsSuccess = false, Message = "Không tìm thấy nhân viên"};

            if (!vaildService.KiemTraNgaySinh(model.DateOfBirth, out var errorMessage))
                return new StatusDTO { IsSuccess = false, Message = errorMessage };

            user.Name = model.Name;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;

            if (imageFile != null)
            {
                user.UrlImage = await vaildService.SaveImage(imageFile, model.Name, model.PhoneNumber, null);
            }

            var result = await employeeRepository.UpdateAsync(user);
            if (!result.Succeeded) return new StatusDTO { IsSuccess = false, Message = "Cập nhật thất bại" };

            return new StatusDTO { IsSuccess = true, Message = "Cập nhật thành công" };
        }

        public async Task<StatusDTO> DeleteAsync(string id)
        {
            var user = await employeeRepository.GetByIdAsync(id);
            if (user == null) return new StatusDTO { IsSuccess = false, Message = "User not found" };

            var result = await employeeRepository.DeleteAsync(user);
            return result.Succeeded ? new StatusDTO { IsSuccess = true, Message = "Xóa nhân viên thành công" } : 
                new StatusDTO { IsSuccess = false, Message = "Xóa nhân viên thất bại" };
        }
    }
}
