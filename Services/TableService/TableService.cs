using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Repositories.TableRepository;
using Ecommerce.Services.JWT;
using Ecommerce.Services.QRImageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Services.TableService
{
    
    public class TableService : ITableService
    {
        private readonly ITableRepository tableRepository;
        private readonly IJwtService jwtService;
        private readonly IQrImageService qrImageService;
        private readonly IConfiguration _config;

        public TableService(ITableRepository tableRepository, IJwtService jwtService, IQrImageService qrImageService, IConfiguration config)
        {
            this.tableRepository = tableRepository;
            this.jwtService = jwtService;
            this.qrImageService = qrImageService;
            _config = config;
        }

        public async Task<StatusDTO> Create(Table model)
        {
            if(model == null) 
                return new StatusDTO { IsSuccess = false, Message = "Model rỗng"};
            if(model.NumberOfSeats < 0)
                return new StatusDTO { IsSuccess = false, Message = "Chỗ ngồi không thể nhỏ hơn 0" };
            var result = await tableRepository.ValidTableName(model.TableName);
            if(!string.IsNullOrEmpty(result))
                return new StatusDTO { IsSuccess = false, Message = result };

            await tableRepository.Create(model);
            return new StatusDTO { IsSuccess = true, Message = $"Tạo bàn: {model.TableName} thành công" };
        }

        //Tạo Jwt bàn và mã QR chứa API xác nhận Jwt
        public async Task<StatusDTO> CreateQRTable(int tableId)
        {
            var token = jwtService.CreateQRTableToken(tableId);

            string address = _config["Global:domain"];
            var qrcodePath = await qrImageService.CreateTableQrAsync(tableId, address + $"page/menu/menu-order.html?token={token}");

            var table = await tableRepository.GetById(tableId);
            table.QRCodePath = qrcodePath;

            await tableRepository.UpdateTableQr(table);
            return new StatusDTO { IsSuccess = true, Message = "Tạo QR bàn thành công" };
        }

        //Xác thực token bàn
        [Authorize(Roles="User")]
        public async Task<QrResolveResult> ValidTableToken(string token, ClaimsPrincipal user)
        {
            if (string.IsNullOrEmpty(token)) return new QrResolveResult { IsValid = false, Message = "Mã QR lỗi" };

            //Decode token và kiểm tra
            var result = jwtService.ValidateQRTableToken(token);
            if (result.IsValid == false) return new QrResolveResult { IsValid = false, Message = result.Message };

            if (user == null)
                return new QrResolveResult { IsValid = false, Message = "Không xác định được người dùng" };

            var userId = user.FindFirstValue("userId");

            //Kiểm tra bàn có tồn tại không
            var table = await tableRepository.GetById(result.TableId);
            if (table == null) return new QrResolveResult { IsValid = false, Message = "Bàn không tồn tại" };

            if (table.TableStatus == "Đang sử dụng" && table.OwnerTable != userId)
                return new QrResolveResult { IsValid = false, Message = "Bàn đang được người khác sử dụng" };

            //Cập nhật chủ bàn cho lần đầu quét
            if (string.IsNullOrEmpty(table.OwnerTable))
            {
                await tableRepository.UpdateTableOwner(userId, table.TableId);
                return new QrResolveResult { IsValid = true, Message = "Xác thực thành công", TableId = result.TableId };
            }

            //Nếu bàn có chủ trùng người đăng nhập
            if (table.OwnerTable == userId)
            {
                return new QrResolveResult
                {
                    IsValid = true,
                    TableId = table.TableId,
                    Message = "Xác thực thành công"
                };
            }

            return new QrResolveResult{IsValid = false, Message = "Không thể xác thực bàn. Vui lòng thử lại."};
        }

        public async Task<StatusDTO> Delete(int id)
        {
            if (id <= 0)
                return new StatusDTO { IsSuccess = false, Message = "Mã bàn không hợp lệ" };

            var table = await tableRepository.GetById(id);
            if (table == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy bàn cần xóa" };

            // ✅ Kiểm tra trạng thái bàn
            if (table.TableStatus == "Đang sử dụng")
                return new StatusDTO
                {
                    IsSuccess = false,
                    Message = "Bàn đang được sử dụng, không thể xóa được."
                };

            await tableRepository.Delete(id);
            return new StatusDTO { IsSuccess = true, Message = "Đã xóa bàn thành công." };
        }



        public async Task<IEnumerable<Table>> GetAll() => await tableRepository.GetAll();

        public async Task<Table> GetById(int id) => await tableRepository.GetById(id);

        public async Task<StatusDTO> Update(Table model)
        {
            if (model.TableId <= 0)
                return new StatusDTO { IsSuccess = false, Message = "Mã bàn không hợp lệ" };
            var table = await tableRepository.GetById(model.TableId);
            if (table == null)
                return new StatusDTO { IsSuccess = false, Message = "Không tìm thấy bàn cần cập nhật" };
            if (model.NumberOfSeats < 0)
                return new StatusDTO { IsSuccess = false, Message = "Chỗ ngồi không thể nhỏ hơn hoặc bằng 0" };
            await tableRepository.Update(model);
            return new StatusDTO { IsSuccess = true, Message = $"Cập nhật bàn: {model.TableName} thành công" };
        }
    }
}
