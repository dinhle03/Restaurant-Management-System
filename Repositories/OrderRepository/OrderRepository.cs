using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext db;

        public OrderRepository(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<Order?> GetById(int orderId) => await db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);

        //Hàm để hiện đơn hàng phía nhà hàng
        public async Task<IEnumerable<OrderDTO>> GetAllOrderAsync()
        {
            return await db.Orders
                .Include(o => o.Table)
                .Include(o => o.Customer)
                .Where(o => o.OrderStatus != "Hoàn thành").Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    OrderStatus = o.OrderStatus,
                    OrderDate = o.OrderDate,
                    TableName = o.Table.TableName,
                    TotalAmount = o.TotalAmount,
                })
                .OrderBy(o => o.OrderDate)
                .ToListAsync();
        }

        //Hiện thông tin chi tiết của 1 đơn phía nhà hàng
        public async Task<IEnumerable<OrderDetailDTO>?> GetOrderDetailsByIdAsync(int orderId)
        {
            var details = await db.OrderDetails
            .Include(od => od.FoodSize)
            .ThenInclude(fs => fs.Menu)
            .Where(od => od.OrderId == orderId)
            .Select(od => new OrderDetailDTO
            {
                FoodName = od.FoodSize.Menu.MenuName + " - " + od.FoodSize.FoodName,
                Quantity = od.Count,
                UrlImage = db.FoodImages
                        .Where(i => i.MenuId == od.FoodSize.MenuId && i.MainImage)
                        .Select(i => i.UrlImage)
                        .FirstOrDefault(),
                Price = od.FoodSize.Price,
            })
            .ToListAsync();

            return details;
        }

        //Hàm cập nhật OrderStatus(Trạng thái đơn hàng) phía bếp
        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
                return false;

            order.OrderStatus = newStatus;

            await db.SaveChangesAsync();
            return true;
        }

        //Hàm yêu cầu thanh toán
        public async Task<bool> RequestPaymentAsync(int orderId)
        {
            var order = await db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
                return false;
            
            order.PaymentStatus = "Yêu cầu thanh toán";
            await db.SaveChangesAsync();
            return true;

        }

        //Láy thông tin đơn hàng chưa thanh toán
        public async Task<IEnumerable<OrderUserDTO>> GetOrdersByUserIdAsync(string customerId)
        {
            var orders = await db.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.FoodSize)
                        .ThenInclude(fs => fs.Menu)
                .Include(o => o.Table)
                .Include(o => o.Customer)
                .Where(o => o.CustomerId == customerId &&
                    (o.PaymentStatus == "Chưa thanh toán" || o.PaymentStatus == "Yêu cầu thanh toán"))
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var orderDtos = orders.Select(o => new OrderUserDTO
            {
                OrderId = o.OrderId,
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus,
                OrderDate = o.OrderDate,
                PaymentStatus = o.PaymentStatus,
                TableName = o.Table.TableName,
                CustomerName = o.Customer.Name,
                CustomerDiscout = db.CustomerRanks.FirstOrDefault(r => r.RankId == o.Customer.RankId).RankDiscount,
                OrderDetail = o.OrderDetails.Select(d => new OrderDetailDTO
                {
                    FoodName = $"{d.FoodSize.Menu.MenuName} - {d.FoodSize.FoodName}",
                    Quantity = d.Count,
                    UrlImage = db.FoodImages
                        .Where(i => i.MenuId == d.FoodSize.MenuId && i.MainImage)
                        .Select(i => i.UrlImage)
                        .FirstOrDefault()
                }).ToList()
            });

            return orderDtos;
        }

        public async Task UpdatePaymentStatusOrderAsync(Order order)
        {
            db.Orders.Update(order);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderDTO>> GetCashOrdersAsync()
        {
            return await db.Orders
                .Where(o => o.PaymentMethod == "Tiền mặt" && o.PaymentStatus != "Đã thanh toán")
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDTO
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TableName = o.Table.TableName,
                    TotalAmount = o.TotalAmount,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrderHasPayment()
        {
            return await db.Orders.Where(o => o.PaymentStatus == "Đã thanh toán" &&
                                o.OrderStatus == "Hoàn thành").ToListAsync();                     
        }
    }
}
