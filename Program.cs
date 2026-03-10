using Ecommerce.Data;
using Ecommerce.DTO;
using Ecommerce.HubSocket;
using Ecommerce.Models;
using Ecommerce.Repositories.AccountRepository;
using Ecommerce.Repositories.CartRepository;
using Ecommerce.Repositories.CustomerRepository;
using Ecommerce.Repositories.DiscountCustomerRepository;
using Ecommerce.Repositories.DiscountRepository;
using Ecommerce.Repositories.EmployeeRepository;
using Ecommerce.Repositories.FoodSizeRepository;
using Ecommerce.Repositories.InventoryRepository;
using Ecommerce.Repositories.MenuCategoryRepository;
using Ecommerce.Repositories.MenuRepository;
using Ecommerce.Repositories.OrderRepository;
using Ecommerce.Repositories.OTP;
using Ecommerce.Repositories.RankAccount;
using Ecommerce.Repositories.RevenueRepository;
using Ecommerce.Repositories.TableRepository;
using Ecommerce.Services;
using Ecommerce.Services.Account;
using Ecommerce.Services.CartService;
using Ecommerce.Services.Customer;
using Ecommerce.Services.DiscountCustomerService;
using Ecommerce.Services.DiscountService;
using Ecommerce.Services.EmployeeService;
using Ecommerce.Services.FoodSizeService;
using Ecommerce.Services.InventoryService;
using Ecommerce.Services.JWT;
using Ecommerce.Services.Mail;
using Ecommerce.Services.MenuCategoryService;
using Ecommerce.Services.MenuService;
using Ecommerce.Services.MomoService;
using Ecommerce.Services.OrderService;
using Ecommerce.Services.PaymentService;
using Ecommerce.Services.QRImageService;
using Ecommerce.Services.RankAccount;
using Ecommerce.Services.RevenueService;
using Ecommerce.Services.TableService;
using Ecommerce.Services.Vaild;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json; // ✅ thêm namespace này

namespace Ecommerce
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins("http://127.0.0.1:5500")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // ✅ Bật camelCase JSON cho toàn bộ API (rất quan trọng cho Flutter)
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecommerce API", Version = "v1" });

                // Nếu dùng JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Nhập token theo dạng: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // SignalR
            builder.Services.AddSignalR();


            //Cấu hình Identity
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<Users, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // Cấu hình JWT

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection("Jwt");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["token"];
                        var path = context.HttpContext.Request.Path;

                        // ✅ Nhận token cho SignalR hub
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/orderHub"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            Console.WriteLine("⚠️ Token đã hết hạn");
                        }
                        else
                        {
                            Console.WriteLine($"❌ Lỗi xác thực token: {context.Exception.Message}");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));

            //== Repository Pattern ==
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IRankAccountRepository, RankAccountRepository>();
            builder.Services.AddScoped<IOTPRepository, OTPRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IMenuCategoryRepository, MenuCategoryRepository>();
            builder.Services.AddScoped<IRankAccountRepository, RankAccountRepository>();
            builder.Services.AddScoped<ITableRepository, TableRepository>();
            builder.Services.AddScoped<IMenuRepository, MenuRepository>();
            builder.Services.AddScoped<IFoodSizeRepository, FoodSizeRepository>();
            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
            builder.Services.AddScoped<IDiscountCustomerRepository, DiscountCustomerRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IRevenueRepository, RevenueRepository>();

            //== Services ==
            builder.Services.AddScoped<IVaildService, VaildService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IMenuCategoryService, MenuCategoryService>();
            builder.Services.AddScoped<IRankAccountService, RankAccountService>();
            builder.Services.AddScoped<ITableService, TableService>();
            builder.Services.AddScoped<IQrImageService, QrImageService>();
            builder.Services.AddScoped<IMenuService, MenuService>();
            builder.Services.AddScoped<IFoodSizeService, FoodSizeService>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IDiscountService, DiscountService>();
            builder.Services.AddScoped<IDiscountCustomerService, DiscountCustomerService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IRevenueService, RevenueService>();
            builder.Services.AddScoped<IMomoService, MomoService>();

            //== Singleton Pattern ==
            builder.Services.AddSingleton<IMailService, MailService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            await RoleConstructor.PhanQuen(app.Services);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            

            app.MapHub<OrderHub>("/orderHub");

            app.MapControllers();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}