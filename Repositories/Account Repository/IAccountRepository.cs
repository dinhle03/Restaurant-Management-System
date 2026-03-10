using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using static System.Net.WebRequestMethods;

namespace Ecommerce.Repositories.AccountRepository
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CreateAccountAsync(RegisterViewModel customer, int rankId, string urlImage, string role);
        Task<(string userName, string role)> GetUserandRole(string email);
        Task<Users> FindUserByEmail(string email);
    }
}
