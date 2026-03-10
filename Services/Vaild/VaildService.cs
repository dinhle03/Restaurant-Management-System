using Ecommerce.Data;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using static System.Net.Mime.MediaTypeNames;

namespace Ecommerce.Services.Vaild
{
    public class VaildService : IVaildService
    {
        private readonly AppDbContext appDbContext;
        private readonly string _baseUploadFolder = "wwwroot/Images/";

        public VaildService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        public async Task<string> SaveImage(IFormFile image, string tenKhachHang, string soDienThoai, string subFolder)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("File ảnh không hợp lệ.");
            }

            if (subFolder.IsNullOrEmpty())
            {
                subFolder = "UserAvatar";
            }

            // Chuẩn hóa tên file: loại bỏ dấu và khoảng trắng
            string fileNameWithoutExt = RemoveDiacritics(tenKhachHang).Replace(" ", "") + soDienThoai;

            // Lấy phần mở rộng của file
            string extension = Path.GetExtension(image.FileName).ToLower();

            // Kiểm tra định dạng ảnh hợp lệ
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException("Định dạng file không hợp lệ. Chỉ chấp nhận JPG, PNG, GIF.");
            }

            // Tạo tên file an toàn
            string safeFileName = fileNameWithoutExt + extension;

            // Tạo thư mục lưu ảnh
            string uploadFolder = Path.Combine(_baseUploadFolder, subFolder);
            string savePath = Path.Combine(uploadFolder, safeFileName);

            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // Lưu file
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            // Trả về đường dẫn tương đối
            return $"/Images/{subFolder}/{safeFileName}";
        }

        public string RemoveDiacritics(string text)
        {
            string normalized = text.Normalize(NormalizationForm.FormD);
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            return regex.Replace(normalized, "").Replace("đ", "d").Replace("Đ", "D");
        }

        //Hàm ktra tính ngày sinh
        public bool KiemTraNgaySinh(DateTime? ngaySinh, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!ngaySinh.HasValue)
            {
                errorMessage = "Ngày sinh không được để trống.";
                return false;
            }

            DateTime today = DateTime.Today;
            DateTime ngaySinhValue = ngaySinh.Value;

            int age = today.Year - ngaySinhValue.Year;

            if (ngaySinhValue > today)
            {
                errorMessage = "Ngày sinh không được là tương lai.";
                return false;
            }
            if (ngaySinhValue.AddYears(10) > today)
            {
                errorMessage = "Tuổi phải lớn hơn hoặc bằng 10.";
                return false;
            }
            if (ngaySinhValue.AddYears(100) < today)
            {
                errorMessage = "Tuổi không được quá 100.";
                return false;
            }

            return true;
        }

        //Kiểm tra Email có trùng lặp không
        public bool checkEmail(string Email)
        {
            var checkKH = appDbContext.Customers.FirstOrDefault(h => h.Email == Email);
            var checkNV = appDbContext.Employees.FirstOrDefault(h => h.Email == Email);
            if (checkKH == null && checkNV == null)
            {
                return true;
            }
            return false;
        }

        public bool checkPhone(string Phone)
        {
            var check = appDbContext.Customers.FirstOrDefault(c => c.PhoneNumber == Phone);
            if(check != null) return true;
            return false;
        }
    }
}
