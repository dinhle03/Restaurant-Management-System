using QRCoder;

namespace Ecommerce.Services.QRImageService
{
    public class QrImageService : IQrImageService
    {
        private readonly IWebHostEnvironment _env;

        public QrImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> CreateTableQrAsync(int tableId, string qrContent)
        {
            var folderPath = Path.Combine(_env.WebRootPath, "Images", "TableQR");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var timestamp = DateTime.UtcNow.Ticks;
            var fileName = $"{tableId}_{timestamp}.png";
            var filePath = Path.Combine(folderPath, fileName);

            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
            var pngQr = new PngByteQRCode(data);
            var qrBytes = pngQr.GetGraphic(20);

            await File.WriteAllBytesAsync(filePath, qrBytes);

            // 🔥 Xóa ảnh QR cũ cùng tableId
            var oldFiles = Directory.GetFiles(folderPath, $"{tableId}_*.png");
            foreach (var old in oldFiles.Where(f => f != filePath))
                File.Delete(old);

            return $"/Images/TableQR/{fileName}";
        }

    }
}
