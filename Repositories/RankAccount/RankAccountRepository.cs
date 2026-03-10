using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.RankAccount
{
    public class RankAccountRepository : IRankAccountRepository
    {
        private readonly AppDbContext db;

        public RankAccountRepository(AppDbContext db)
        {
            this.db = db;
        }

        public async Task CreateAsync(CustomerRank model)
        {
            var customerRank = new CustomerRank
            {
                RankName = model.RankName,
                RankDiscount = model.RankDiscount,
                RankPoint = model.RankPoint,
            };
            db.CustomerRanks.Add(customerRank);
            await db.SaveChangesAsync();
            // Sau khi thêm hạng mới, cập nhật lại toàn bộ hạng của khách hàng
            await UpdateCustomerRanksByPoint();
        }

        public async Task DeleteAsync(int id)
        {
            //var customerRank = await db.CustomerRanks.FindAsync(id);
            //db.CustomerRanks.Remove(customerRank);
            //await db.SaveChangesAsync();
            var rankToDelete = await db.CustomerRanks.FindAsync(id);
            if (rankToDelete == null)
                throw new Exception("Không tìm thấy hạng cần xóa");
            var customersInRank = await db.Customers
                .Where(c => c.RankId == id)
                .ToListAsync();

            if (customersInRank.Any())
            {
                var otherRanks = await db.CustomerRanks
                    .Where(r => r.RankId != id)
                    .OrderBy(r => r.RankPoint)
                    .ToListAsync();

                if (!otherRanks.Any())
                    throw new Exception("Không thể xoá vì không còn hạng nào khác để chuyển khách hàng.");

                foreach (var customer in customersInRank)
                {

                    var newRank = otherRanks.LastOrDefault(r => r.RankPoint <= customer.Point);

                    customer.RankId = newRank?.RankId ?? otherRanks.First().RankId;
                }

                await db.SaveChangesAsync();
            }

            db.CustomerRanks.Remove(rankToDelete);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<CustomerRank>> GetAllAsync()
        {
            var lst = await db.CustomerRanks.ToListAsync();
            return lst;
        }

        public async Task<CustomerRank> GetByIdAsync(int id)
        {
            var customerRank = await db.CustomerRanks.FindAsync(id);
            return customerRank;
        }

        public async Task<int?> GetRankPointByUserIdAsync(string userId)
        {
            var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == userId);
            if (customer == null)
                return null; // hoặc throw exception tùy bạn

            var rankPoint = await db.CustomerRanks
                .Where(r => r.RankId == customer.RankId)
                .Select(r => (int?)r.RankDiscount)
                .FirstOrDefaultAsync();

            return rankPoint;
        }


        public async Task<int> SelectLastestRank()
        {
            var rankId = await db.CustomerRanks.OrderBy(cr => cr.RankPoint).Select(cr => cr.RankId).FirstOrDefaultAsync();
            if (rankId == 0)
            {
                CustomerRank csItem = new CustomerRank
                {
                    RankName = "Chưa có hạng",
                    RankPoint = 0,
                    RankDiscount = 0,
                };
                db.CustomerRanks.Add(csItem);
                await db.SaveChangesAsync();
                rankId = csItem.RankId;

            }
            return rankId;
        }

        public async Task UpdateAsync(CustomerRank model)
        {
            var customerRank = await GetByIdAsync(model.RankId);

            customerRank.RankPoint = model.RankPoint;
            customerRank.RankDiscount = model.RankDiscount;
            customerRank.RankName = model.RankName;

            db.CustomerRanks.Update(customerRank);
            await db.SaveChangesAsync();

            // Sau khi chỉnh sửa, cập nhật lại hạng cho toàn bộ khách hàng
            await UpdateCustomerRanksByPoint();
        }

        // HÀM TỰ CẬP NHẬT HẠNG KHÁCH HÀNG THEO ĐIỂM
        private async Task UpdateCustomerRanksByPoint()
        {
            var ranks = await db.CustomerRanks
                .OrderBy(r => r.RankPoint)
                .ToListAsync();

            if (!ranks.Any()) return;

            var customers = await db.Customers.ToListAsync();

            foreach (var customer in customers)
            {
                var newRank = ranks.LastOrDefault(r => r.RankPoint <= customer.Point);

                if (newRank != null && customer.RankId != newRank.RankId)
                {
                    customer.RankId = newRank.RankId;
                }
            }

            await db.SaveChangesAsync();
        }
    }
}