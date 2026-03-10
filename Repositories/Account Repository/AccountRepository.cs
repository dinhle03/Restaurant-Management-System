using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Repositories.CustomerRepository;
using Ecommerce.Services.JWT;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ecommerce.Repositories.AccountRepository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<Users> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ICustomerRepository customerRepository;


        public AccountRepository(UserManager<Users> userManager, RoleManager<IdentityRole> roleManager, ICustomerRepository customerRepository)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.customerRepository = customerRepository;
        }

        public async Task<IdentityResult> CreateAccountAsync(RegisterViewModel model, int rankId, string urlImage, string role)
        {
            if(role == "User")
            {
                var result = await customerRepository.CreateAsync(model, urlImage, rankId, role);
            }

            return IdentityResult.Success;
        }

        public async Task<(string userName, string role)> GetUserandRole(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return ("", "");

            //Phương thức này sẽ trả về các role của 1 user
            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (role == null) return (user.Name, "");
            return (user.Name, role);
        }

        public async Task<Users> FindUserByEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            return user;
        }
    }
}
