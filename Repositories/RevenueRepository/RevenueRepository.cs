using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.RevenueRepository
{
    public class RevenueRepository : IRevenueRepository
    {
        private readonly AppDbContext db;

        public RevenueRepository(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<Revenue?> GetRevenueByIdAsync(int revenueId)
        {
            return await db.Revenues.FirstOrDefaultAsync(r => r.RevenueId == revenueId);
        }

        public async Task UpdateRevenueAsync(Revenue revenue)
        {
            db.Revenues.Update(revenue);
            await db.SaveChangesAsync();
        }

        private async Task<int> CreateRevenue()
        {
            var revenue = new Revenue
            {
                TotalAmount = 0,
                Date = DateTime.Now.Date,
            };

            db.Revenues.Add(revenue);
            await db.SaveChangesAsync();

            return revenue.RevenueId;
        }

        public async Task<int> CheckRevenue()
        {
            DateTime date = DateTime.Now.Date;
            var rev = await db.Revenues.FirstOrDefaultAsync(r => r.Date == date);
            if (rev != null)
            {
                return rev.RevenueId;
            }

            int revId = await CreateRevenue();
            return revId;
        }

        public async Task addRevenue_Orders(int orderId, int revenueId)
        {
            var item = new Revenue_Order
            {
                RevenueId = revenueId,
                OrderId = orderId
            };
            db.Revenue_Orders.Add(item);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Revenue>> GetAllRevenues()
        {
            return await db.Revenues.ToListAsync();
        }

        public async Task<object> GetNumberOfUserandRevenueOfDay()
        {
            int NumberOfCustomser = db.Customers.Count();
            int NumberOfEmployee = db.Employees.Count();
            var today = DateTime.Today;
            decimal TotalOfOrderInDay = await db.Revenues.Where(r => r.Date.Date == today).Select(r => r.TotalAmount).FirstOrDefaultAsync();

            return new
            {
                NumberOfCustomser,
                NumberOfEmployee,
                TotalOfOrderInDay,
            };
        }
    }
}
