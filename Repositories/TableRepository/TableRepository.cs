using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;


namespace Ecommerce.Repositories.TableRepository
{
    public class TableRepository : ITableRepository
    {
        private readonly AppDbContext db;

        public TableRepository(AppDbContext db)
        {
            this.db = db;
        }

        public async Task Create(Table model)
        {
            var table = new Table
            {
                TableName = model.TableName,
                TableStatus = "Trống",
                NumberOfSeats = model.NumberOfSeats,
                QRCodePath = "",
            };
            db.Tables.Add(table);
            await db.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var table = await GetById(id);
            if (!string.IsNullOrEmpty(table.QRCodePath))
            {
                DeleteTableQrImage(table.QRCodePath);
            }
            db.Tables.Remove(table);
            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Table>> GetAll()
        {
            var lst = await db.Tables.ToListAsync();
            return lst;
        }

        public async Task<Table> GetById(int id)
        {
            var table = await db.Tables.FindAsync(id);
            return table;
        }

        public async Task Update(Table model)
        {
            var table = await GetById(model.TableId);

            table.TableName = model.TableName;
            table.TableStatus = model.TableStatus;
            table.NumberOfSeats = model.NumberOfSeats;
            if (model.TableStatus == "Trống")
            {
                table.OwnerTable = "";
                if (!string.IsNullOrEmpty(table.QRCodePath))
                {
                    DeleteTableQrImage(table.QRCodePath);
                    table.QRCodePath = null;
                }
            }
            else
                table.OwnerTable = model.OwnerTable;

            await db.SaveChangesAsync();
        }

        private void DeleteTableQrImage(string relativePath)
        {
            try
            {
                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var cleanPath = relativePath.TrimStart('/'); // bỏ dấu / ở đầu nếu có
                var fullPath = Path.Combine(rootPath, cleanPath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    Console.WriteLine($"✅ Đã xóa QR Code: {fullPath}");
                }
                else
                {
                    Console.WriteLine($"⚠️ Không tìm thấy file QR Code tại: {fullPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi xóa QR Code: {ex.Message}");
            }
        }

        public async Task UpdateTableOwner(string? userId, int tableId)
        {
            var table = await GetById(tableId);

            table.OwnerTable = userId;
            table.TableStatus = "Đang sử dụng";

            await db.SaveChangesAsync();
        }

        public async Task<string> ValidTableName(string tableName)
        {
            var check = await db.Tables.FirstOrDefaultAsync(t => t.TableName == tableName);
            if (check != null) return "Tên bàn bị trùng";
            return "";
        }

        public async Task UpdateTableQr(Table model)
        {
            db.Tables.Update(model);
            await db.SaveChangesAsync();
        }
    }
}
