using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Repositories.CustomerRepository;
using Ecommerce.Services.Vaild;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository customerRepository;
        private readonly UserManager<Users> userManager;

        public CustomerService(ICustomerRepository customerRepository, UserManager<Users> userManager)
        {
            this.customerRepository = customerRepository;
            this.userManager = userManager;
        }

        public async Task<List<CustomerDTO>> GetAll(string? search = null)
        {
            var lstCustomer = await customerRepository.GetAll(search);
            return lstCustomer;
        }

        public async Task<CustomerResultDTO> GetById(string userId)
        {
            var customerItem = await customerRepository.GetById(userId);
            if (customerItem == null)
            {
                return new CustomerResultDTO
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy khách hàng."
                };
            }

            return new CustomerResultDTO
            {
                IsSuccess = true,
                Message = "Lấy thông tin thành công.",
                Customer = new CustomerDTO
                {
                    CustomerId = customerItem.Id,
                    Name = customerItem.Name,
                    Email = customerItem.Email,
                    PhoneNumber = customerItem.PhoneNumber,
                    Gender = customerItem.Gender,
                    DateOfBirth = customerItem.DateOfBirth,
                    UrlImage = customerItem.UrlImage,
                    RankName = customerItem.CustomerRank.RankName,
                    RankId = customerItem.RankId,
                    Point = customerItem.Point,
                }
            };
        }

        public async Task<StatusDTO> Update(UpdateCustomerViewModel model, string userId, IFormFile? UrlImage)
        {
            var customer = await customerRepository.GetById(userId);
            if (customer == null)
                return new StatusDTO { IsSuccess = false, Message = $"Không tìm thấy khách hàng với ID: {userId}" };

            // Kiểm tra email
            var existingUserByEmail = await userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null && model.Email != customer.Email)
                return new StatusDTO { IsSuccess = false, Message = "Email đã tồn tại" };

            // Kiểm tra số điện thoại
            var existingPhoneCustomer = await customerRepository.GetCustomerByPhoneAsync(model.PhoneNumber, customer.Id);
            if (existingPhoneCustomer != null)
                return new StatusDTO { IsSuccess = false, Message = "Số điện thoại đã tồn tại" };
            var customerItem = await customerRepository.Update(model, userId, UrlImage);

            if (customerItem == null) return new StatusDTO { IsSuccess = false, Message = "Không cập nhật được thông tin Customer" };

            return new StatusDTO { IsSuccess = true, Message = "Cập nhật thành công" };
        }

        public async Task<StatusDTO> Delete(string userId)
        {
            var customer = await customerRepository.GetById(userId);
            if (customer == null)
                return new StatusDTO { IsSuccess = false, Message = $"Không tìm thấy khách hàng với ID: {userId}" };

            var hasOrders = await customerRepository.IsCustomerCart(userId);
            if (hasOrders)
                return new StatusDTO { IsSuccess = false, Message = "Không thể xoá khách hàng đã có đơn hàng!" };

            var result = await customerRepository.Delete(customer);
            if (result.IsSuccess == false)
            {
                return new StatusDTO { IsSuccess = false, Message = result.Message };
            }
            return new StatusDTO { IsSuccess = true, Message = "Xoá khách hàng thành công" };
        }

        public async Task<StatusDTO> LockUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy người dùng." };

            user.LockoutEnd = DateTimeOffset.MaxValue;
            await userManager.UpdateAsync(user);

            return new StatusDTO { IsSuccess = true, Message = "Khóa tài khoản thành công" };
        }

        public async Task<StatusDTO> UnlockUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy người dùng." };

            user.LockoutEnd = null;
            await userManager.UpdateAsync(user);

            return new StatusDTO { IsSuccess = true, Message = "Mở khóa tài khoản thành công" };
        }
        public async Task<List<CustomerDTO>> GetLockedCustomersAsync()
        {
            var allCustomers = await customerRepository.GetAll();
            var lockedCustomers = allCustomers
                .Where(c => c.IsLocked)
                .ToList();

            return lockedCustomers;
        }
    }
}