using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.DiscountRepository
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly AppDbContext context;
        public DiscountRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Discount>> GetAll(string? search = null)
        {
            var query = context.Discounts.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => x.DiscountName.Contains(search));
            return await query.OrderByDescending(x => x.DateStart).ToListAsync();
        }

        public async Task<Discount?> GetById(string id)
        {
            return await context.Discounts.FirstOrDefaultAsync(x => x.DiscountId == id);
        }

        public async Task Create(Discount model)
        {
            context.Discounts.Add(model);
            await context.SaveChangesAsync();
        }

        public async Task Update(Discount model)
        {
            var exist = await context.Discounts.FirstOrDefaultAsync(x => x.DiscountId == model.DiscountId);
            if (exist == null)
                throw new Exception("Không tìm thấy mã giảm giá để cập nhật.");

            exist.DiscountName = model.DiscountName;
            exist.DiscountCategory = model.DiscountCategory;
            exist.DiscountPrice = model.DiscountPrice;
            exist.DateStart = model.DateStart;
            exist.DateEnd = model.DateEnd;
            exist.DiscountStatus = model.DiscountStatus;
            exist.RequiredPoints = model.RequiredPoints; // ✅ thêm dòng này

            await context.SaveChangesAsync();
        }


        public async Task Delete(string id)
        {
            var item = await context.Discounts.FindAsync(id);
            if (item != null)
            {
                context.Discounts.Remove(item);
                await context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExchangeDiscountAsync(string discountId, string customerId)
        {
            var discount = await context.Discounts.FirstOrDefaultAsync(d => d.DiscountId == discountId);
            var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);

            if (discount == null || customer == null)
                throw new Exception("Không tìm thấy dữ liệu hợp lệ.");

            if (customer.Point < discount.RequiredPoints)
                throw new Exception("Không đủ điểm để đổi mã này.");

            // Kiểm tra đã có mã chưa
            var exist = await context.Discount_Customers
                .AnyAsync(dc => dc.CustomerId == customerId && dc.DiscountId == discountId);

            if (exist)
                throw new Exception("Bạn đã đổi mã này rồi.");

            // Trừ điểm và tạo bản ghi mới
            customer.Point -= discount.RequiredPoints;

            context.Discount_Customers.Add(new Discount_Customer
            {
                DiscountId = discountId,
                CustomerId = customerId,
                isUsed = false
            });

            await context.SaveChangesAsync();
            return true;
        }

        //Lấy thông tin mã giảm giá mà user có\
        public async Task<Discount_Customer?> GetDiscountByUserId(string userId, string discountId)
        {
            var discount_customer = await context.Discount_Customers.FirstOrDefaultAsync(dc => dc.CustomerId == userId && dc.DiscountId == discountId);
            if (discount_customer == null) return null;

            return discount_customer;
        } 

    }
}
