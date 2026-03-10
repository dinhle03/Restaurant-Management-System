using Ecommerce.DTO;
using Ecommerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.Services.JWT
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<Users> _userManager;

        public JwtService(IConfiguration config, UserManager<Users> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<string> CreateToken(Users user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim("userId", user.Id),
                new Claim("userName", user.UserName),
                new Claim("email", user.Email),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateQRTableToken(int tableId, int minutes = 15)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtQRTable:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
            new Claim("tableId", tableId.ToString()),
            new Claim("type", "qr")
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //Xác thực token từ QR bàn
        public QrResolveResult ValidateQRTableToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var result = new QrResolveResult();

            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtQRTable:Secret"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = handler.ValidateToken(token, parameters, out var validated);
                var tableId = int.Parse(principal.FindFirstValue("tableId")!);

                result.IsValid = true;
                result.TableId = tableId;
            }
            catch (SecurityTokenExpiredException)
            {
                result.IsValid = false;
                result.Message = "Token QR đã hết hạn.";
            }
            catch (Exception)
            {
                result.IsValid = false;
                result.Message = "Token QR không hợp lệ.";
            }

            return result;
        }
    }
}
