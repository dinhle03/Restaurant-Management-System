namespace Ecommerce.Services.Vaild
{
    public interface IVaildService
    {
        Task<string> SaveImage(IFormFile image, string tenKhachHang, string soDienThoai, string subFolder);
        string RemoveDiacritics(string text);
        bool KiemTraNgaySinh(DateTime? ngaySinh, out string errorMessage);
        bool checkEmail(string Email);
        bool checkPhone(string Phone);
    }
}
