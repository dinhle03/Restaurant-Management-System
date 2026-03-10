using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Repositories.DiscountRepository;
using Ecommerce.Repositories.DiscountCustomerRepository;

namespace Ecommerce.Services.DiscountService
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository discountRepository;
        private readonly IDiscountCustomerRepository discountCustomerRepository;

        public DiscountService(
            IDiscountRepository discountRepository,
            IDiscountCustomerRepository discountCustomerRepository)
        {
            this.discountRepository = discountRepository;
            this.discountCustomerRepository = discountCustomerRepository;
        }

        public async Task<IEnumerable<Discount>> GetAll(string? search = null)
            => await discountRepository.GetAll(search);

        public async Task<Discount?> GetById(string id)
            => await discountRepository.GetById(id);

        public async Task<StatusDTO> Create(Discount model)
        {
            if (string.IsNullOrWhiteSpace(model.DiscountId))
                model.DiscountId = Guid.NewGuid().ToString();

            if (model.DateEnd <= model.DateStart)
                return new StatusDTO { IsSuccess = false, Message = "Ngày kết thúc phải sau ngày bắt đầu" };

            await discountRepository.Create(model);
            return new StatusDTO { IsSuccess = true, Message = "Thêm mã giảm giá thành công" };
        }

        public async Task<StatusDTO> Update(Discount model)
        {
            var exist = await discountRepository.GetById(model.DiscountId);
            if (exist == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy mã giảm giá" };

            await discountRepository.Update(model);
            return new StatusDTO { IsSuccess = true, Message = "Cập nhật mã giảm giá thành công" };
        }

        public async Task<StatusDTO> Delete(string id)
        {
            var exist = await discountRepository.GetById(id);
            if (exist == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy mã giảm giá để xóa" };

            await discountRepository.Delete(id);
            return new StatusDTO { IsSuccess = true, Message = "Xóa mã giảm giá thành công" };
        }

        public async Task<StatusDTO> AddDiscountToCustomer(string discountId, string customerId)
        {
            var exist = await discountRepository.GetById(discountId);
            if (exist == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy mã giảm giá" };

            var added = await discountCustomerRepository.AddDiscountToCustomer(discountId, customerId);
            if (!added)
                return new StatusDTO { IsSuccess = false, Message = "Khách hàng đã nhận mã này rồi" };

            return new StatusDTO { IsSuccess = true, Message = "Nhận mã giảm giá thành công" };
        }

        // ✅ CHUẨN 3 LỚP: Gọi repository để xử lý Exchange Discount
        public async Task<StatusDTO> ExchangeDiscount(string discountId, string customerId)
        {
            try
            {
                var success = await discountRepository.ExchangeDiscountAsync(discountId, customerId);
                return new StatusDTO
                {
                    IsSuccess = success,
                    Message = success ? "✅ Đổi mã giảm giá thành công!" : "❌ Đổi mã thất bại."
                };
            }
            catch (Exception ex)
            {
                return new StatusDTO
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}
