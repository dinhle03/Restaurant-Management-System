using Azure;
using Ecommerce.DTO;
using Ecommerce.HubSocket;
using Ecommerce.Repositories.OrderRepository;
using Ecommerce.Repositories.RevenueRepository;
using Ecommerce.Services.MomoService;
using Ecommerce.Services.TableService;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;

namespace Ecommerce.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IMomoService momoService;
        private readonly IOrderRepository orderRepository;
        private readonly IRevenueRepository revenueRepository;
        private readonly IHubContext<OrderHub> hubContext;
        private readonly ITableService tableService;

        public PaymentService(IMomoService momoService, IOrderRepository orderRepository, IRevenueRepository revenueRepository, IHubContext<OrderHub> hubContext, ITableService tableService)
        {
            this.momoService = momoService;
            this.orderRepository = orderRepository;
            this.revenueRepository = revenueRepository;
            this.hubContext = hubContext;
            this.tableService = tableService;
        }

        public async Task<string> CreatePaymentUrlMomo(OrderInfo model)
        {
            var response = await momoService.CreatePaymentAsync(model);
            return response.PayUrl;
        }

        public async Task<object> PaymentCallbackAsync(IQueryCollection query)
        {
            var response = momoService.PaymentExecute(query);

            if (query == null || !query.Any())
            {
                return new { success = false, message = "Không nhận được dữ liệu callback từ MoMo." };
            }

            if (query["resultCode"].ToString() != "0")
            {
                return new { success = false, message = "Giao dịch không thành công!" };
            }

            int orderIdInput;
            try
            {
                string rawOrderId = response.OrderId;

                if (rawOrderId.Contains("-"))
                {
                    string[] parts = rawOrderId.Split('-');
                    orderIdInput = int.Parse(parts[0]);
                }
                else
                {
                    orderIdInput = int.Parse(rawOrderId);
                }
            }
            catch (Exception)
            {
                return new { success = false, message = "Lỗi định dạng OrderId trả về." };
            }

            var order = await orderRepository.GetById(orderIdInput);

            if (order == null)
            {
                return new { success = false, message = "Đơn hàng không tồn tại." };
            }

            order.PaymentStatus = "Đã thanh toán";

            int revenueId = await revenueRepository.CheckRevenue();
            var revenue = await revenueRepository.GetRevenueByIdAsync(revenueId);
            if (revenue != null)
            {
                revenue.TotalAmount += order.TotalAmount;
                await revenueRepository.addRevenue_Orders(order.OrderId, revenueId);
                await revenueRepository.UpdateRevenueAsync(revenue);
            }

            var table = await tableService.GetById(order.TableId);
            table.TableStatus = "Trống";
            await tableService.Update(table);

            await orderRepository.UpdatePaymentStatusOrderAsync(order);

            var orderDetail = await orderRepository.GetOrderDetailsByIdAsync(order.OrderId);

            return new
            {
                success = true,
                message = "Thanh toán thành công!",
                orderDetail
            };
        }

        public async Task<object> ConfirmOrderPayByCash(int orderId)
        {
            var order = await orderRepository.GetById(orderId);
            if (order == null)
            {
                return new { success = false, message = "Đơn hàng không tồn tại." };
            }

            // Cập nhật trạng thái đơn hàng
            order.PaymentStatus = "Đã thanh toán";

            // Lấy và cập nhật doanh thu
            int revenueId = await revenueRepository.CheckRevenue();
            var revenue = await revenueRepository.GetRevenueByIdAsync(revenueId);
            if (revenue != null)
            {
                revenue.TotalAmount += order.TotalAmount;
                await revenueRepository.addRevenue_Orders(order.OrderId, revenueId);
                await revenueRepository.UpdateRevenueAsync(revenue);
            }

            await orderRepository.UpdatePaymentStatusOrderAsync(order);

            var orderDetail = await orderRepository.GetOrderDetailsByIdAsync(order.OrderId);

            var table = await tableService.GetById(order.TableId);
            table.TableStatus = "Trống";
            await tableService.Update(table);

            //Gửi socket qua cho User
            var result = new
            {
                Message = $"Thanh toán thành công đơn hàng #{order.OrderId}",
            };

            if (order.CustomerId != null)
            {
                await hubContext.Clients.Group($"User_{order.CustomerId}")
                    .SendAsync("PaymentByCashResult", result);
                Console.WriteLine($"📡 [SignalR] Gửi OrderStatusChanged đến User_{order.CustomerId} | Message= {result}");
            }
            else
            {
                Console.WriteLine($"⚠️ [SignalR] Đơn #{order.OrderId} không có CustomerId, không thể gửi socket!");
            }

            return new
            {
                success = true,
                message = "Thanh toán thành công!",
                orderDetail
            };
        }
    }
}
