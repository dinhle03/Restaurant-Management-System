using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using System.Data;
using Ecommerce.ViewModels;

namespace Ecommerce.Repositories.EmployeeRepository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext db;

        public EmployeeRepository(UserManager<Users> userManager, RoleManager<IdentityRole> roleManager, AppDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.db = db;
        }

        public async Task<IEnumerable<Users>> GetAllAsync(string? search)
        {
            var query = db.Employees.AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.Name.Contains(search) || u.PhoneNumber.Contains(search));
            return await query.ToListAsync();
        }

        public async Task<IdentityResult> CreateAsync(RegisterViewModel model, string urlImage, string role)
        {
            var employee = new Employee
            {
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                UrlImage = urlImage,
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                NormalizedUserName = model.Email.ToUpper(),
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(employee, model.Password);

            if (!result.Succeeded)
                return result;

            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            await _userManager.AddToRoleAsync(employee, role);
            return IdentityResult.Success;
        }

        public async Task<Users?> GetByIdAsync(string id) => await db.Employees.FindAsync(id);

        public async Task<IList<string>> GetRolesAsync(Users user) => await _userManager.GetRolesAsync(user);

        public async Task<IdentityResult> UpdateAsync(Users user) => await _userManager.UpdateAsync(user);

        public async Task<IdentityResult> DeleteAsync(Users user) => await _userManager.DeleteAsync(user);


    }
}
