using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;

namespace Ecommerce.HubSocket
{
    [Authorize]
    public class OrderHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("userId")?.Value;

            // ✅ Thử đọc cả hai kiểu claim (để tương thích cả 2 dạng token)
            var role = Context.User?.FindFirst("role")?.Value
                ?? Context.User?.FindFirst(ClaimTypes.Role)?.Value
                ?? Context.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            Console.WriteLine($"🔌 [Hub] UserId: {userId}, Role: {role} vừa kết nối!");

            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            Console.WriteLine($"📌 [Hub] Đã thêm {userId} vào nhóm User_{userId}");

            if (!string.IsNullOrEmpty(role))
            {
                if (role == "Admin")
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
                    Console.WriteLine($"✅ [Hub] Đã thêm vào nhóm: Admins");
                }
                else if (role == "Staff")
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Staffs");
                    Console.WriteLine($"✅ [Hub] Đã thêm vào nhóm: Staffs");
                }
                else if (role == "User")
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Users");
                    Console.WriteLine($"✅ [Hub] Đã thêm vào nhóm: Users");
                }
            }
            else
            {
                Console.WriteLine("⚠️ [Hub] Không tìm thấy claim 'role' trong token");
            }

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.FindFirst("userId")?.Value;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
            Console.WriteLine($"🔌 [Hub] UserId: {userId} đã ngắt kết nối.");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
