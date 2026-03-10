namespace Ecommerce.Services.QRImageService
{
    public interface IQrImageService
    {
        Task<string> CreateTableQrAsync(int tableId, string qrContent);
    }
}
