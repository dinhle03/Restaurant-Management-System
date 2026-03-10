using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Repositories.CustomerRepository;
using Ecommerce.Repositories.DiscountCustomerRepository;
using Ecommerce.Repositories.DiscountRepository;

namespace Ecommerce.Services.DiscountCustomerService
{
    public class DiscountCustomerService : IDiscountCustomerService
    {
        private readonly IDiscountCustomerRepository _discountCustomerRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IDiscountRepository _discountRepo;

        public DiscountCustomerService(
            IDiscountCustomerRepository discountCustomerRepo,
            ICustomerRepository customerRepo,
            IDiscountRepository discountRepo)
        {
            _discountCustomerRepo = discountCustomerRepo;
            _customerRepo = customerRepo;
            _discountRepo = discountRepo;
        }

        // ✅ Lấy danh sách mã giảm giá của người dùng
        public async Task<IEnumerable<object>> GetUserDiscountsAsync(string customerId)
        {
            var list = await _discountCustomerRepo.GetByCustomerIdAsync(customerId);

            return list.Select(dc => new
            {
                dc.DiscountId,
                dc.CustomerId,
                dc.isUsed,
                discount = new
                {
                    dc.Discount.DiscountName,
                    dc.Discount.DiscountCategory,
                    dc.Discount.DiscountPrice,
                    dc.Discount.DateStart,
                    dc.Discount.DateEnd,
                    dc.Discount.DiscountStatus
                }
            });
        }

        // ✅ Sử dụng mã giảm giá (đánh dấu isUsed = true)
        public async Task<bool> UseDiscountAsync(string customerId, string discountId)
        {
            var record = await _discountCustomerRepo.GetByCustomerAndDiscountAsync(customerId, discountId);
            if (record == null || record.isUsed)
                return false;

            record.isUsed = true;
            await _discountCustomerRepo.UpdateAsync(record);
            return true;
        }

        // ✅ Admin gán mã cho người dùng
        public async Task<bool> AssignDiscountToUserAsync(string discountId, string customerId)
        {
            return await _discountCustomerRepo.AddDiscountToCustomer(discountId, customerId);
        }

        public async Task<StatusDTO> ExchangeDiscountAsync(string customerId, string discountId)
        {
            // 1. Đã đổi chưa
            var existed = await _discountCustomerRepo
                .GetByCustomerAndDiscountAsync(customerId, discountId);

            if (existed != null)
                return new StatusDTO
                {
                    IsSuccess = false,
                    Message = "Bạn đã đổi mã này rồi."
                };

            // 2. Lấy discount
            var discount = await _discountRepo.GetById(discountId);
            if (discount == null || !discount.DiscountStatus)
                return new StatusDTO
                {
                    IsSuccess = false,
                    Message = "Mã giảm giá không tồn tại hoặc đã hết hiệu lực."
                };

            // 3. LẤY ĐIỂM KHÁCH HÀNG (KHÔNG LOAD CUSTOMER)
            var currentPoint = await _discountCustomerRepo.GetCustomerPoint(customerId);

            if (currentPoint < discount.RequiredPoints)
                return new StatusDTO
                {
                    IsSuccess = false,
                    Message = "Không đủ điểm để đổi mã."
                };

            // 4. TRỪ ĐIỂM (UPDATE TRỰC TIẾP)
            await _discountCustomerRepo.DeductPoint(customerId, discount.RequiredPoints);

            // 5. GÁN MÃ GIẢM GIÁ
            await _discountCustomerRepo.AddDiscountToCustomer(discountId, customerId);

            return new StatusDTO
            {
                IsSuccess = true,
                Message = "✅ Đổi mã giảm giá thành công!"
            };
        }
    }
}
