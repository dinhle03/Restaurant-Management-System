using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.ViewModels;

namespace Ecommerce.Services.JWT
{
    public interface IJwtService
    {
        Task<string> CreateToken(Users user);
        QrResolveResult ValidateQRTableToken(string token);
        string CreateQRTableToken(int tableId, int minutes = 15);
    }
}
