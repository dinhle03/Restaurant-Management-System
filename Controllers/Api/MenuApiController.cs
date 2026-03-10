using Ecommerce.DTO;
using Ecommerce.Models;
using Ecommerce.Services.MenuService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Ecommerce.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // ✅ Áp dụng mặc định cho tất cả trừ User API bên dưới
    public class MenuApiController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IWebHostEnvironment _env;

        public MenuApiController(IMenuService menuService, IWebHostEnvironment env)
        {
            _menuService = menuService;
            _env = env;
        }

        // =========================
        // GET: api/MenuApi
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var menus = await _menuService.GetAll(search);
            return Ok(menus);
        }

        // =========================
        // GET: api/MenuApi/{id}
        // =========================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var menu = await _menuService.GetById(id);
            if (menu == null)
                return NotFound(new StatusDTO
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy món ăn"
                });

            return Ok(menu);
        }

        // =========================
        // POST: api/MenuApi (tạo menu + nhiều ảnh)
        // =========================
        [HttpPost]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> Create([FromForm] MenuCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new StatusDTO
                {
                    IsSuccess = false,
                    Message = "Dữ liệu không hợp lệ"
                });
            }

            try
            {
                var menu = new Menu
                {
                    MenuName = dto.MenuName,
                    MenuCategoryId = dto.MenuCategoryId,
                    Detail = dto.Detail,
                    FoodImages = new List<FoodImage>()
                };

                var result = await _menuService.Create(menu);
                if (!result.IsSuccess)
                    return BadRequest(result);

                // ✅ Upload ảnh
                if (dto.Images != null && dto.Images.Any())
                {
                    var uploadPath = Path.Combine(_env.WebRootPath, "images", "menu");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    int index = 0;
                    foreach (var file in dto.Images)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var filePath = Path.Combine(uploadPath, fileName);
                        using var stream = new FileStream(filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        menu.FoodImages.Add(new FoodImage
                        {
                            UrlImage = $"/images/menu/{fileName}",
                            MenuId = menu.MenuId,
                            SortOrder = index,
                            MainImage = index == 0
                        });

                        index++;
                    }

                    await _menuService.Update(menu);
                }

                return Ok(new StatusDTO
                {
                    IsSuccess = true,
                    Message = $"✅ Thêm món '{dto.MenuName}' thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new StatusDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi server: {ex.Message}"
                });
            }
        }

        // =========================
        // PUT: api/MenuApi/{id} (chỉnh sửa menu + ảnh)
        // =========================
        [HttpPut("{id}")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> Update(int id, [FromForm] MenuUpdateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new StatusDTO
                {
                    IsSuccess = false,
                    Message = "Dữ liệu không hợp lệ"
                });
            }

            try
            {
                var existing = await _menuService.GetEntityById(id);
                if (existing == null)
                {
                    return NotFound(new StatusDTO
                    {
                        IsSuccess = false,
                        Message = "Không tìm thấy món ăn"
                    });
                }

                // ✅ Cập nhật cơ bản
                existing.MenuName = dto.MenuName;
                existing.Detail = dto.Detail;
                existing.MenuCategoryId = dto.MenuCategoryId;

                // ✅ Parse JSON: existingImages { keep, main }
                if (!string.IsNullOrEmpty(dto.ExistingImages))
                {
                    using var doc = JsonDocument.Parse(dto.ExistingImages);
                    var root = doc.RootElement;

                    var keepList = new List<string>();
                    if (root.TryGetProperty("keep", out var keepProp))
                    {
                        keepList = keepProp.EnumerateArray()
                            .Select(e => e.GetString())
                            .Where(s => !string.IsNullOrEmpty(s))
                            .Cast<string>()
                            .ToList();
                    }

                    string? mainUrl = null;
                    if (root.TryGetProperty("main", out var mainProp))
                        mainUrl = mainProp.GetString();

                    existing.FoodImages = existing.FoodImages
                        .Where(img => keepList.Contains(img.UrlImage))
                        .ToList();

                    if (mainUrl != null)
                    {
                        foreach (var img in existing.FoodImages)
                            img.MainImage = img.UrlImage == mainUrl;
                    }
                }

                // ✅ Upload ảnh mới
                if (dto.Images != null && dto.Images.Any())
                {
                    var uploadPath = Path.Combine(_env.WebRootPath, "images", "menu");
                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    int sortOrder = existing.FoodImages.Count;
                    foreach (var file in dto.Images)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var filePath = Path.Combine(uploadPath, fileName);
                        using var stream = new FileStream(filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        existing.FoodImages.Add(new FoodImage
                        {
                            UrlImage = $"/images/menu/{fileName}",
                            MenuId = id,
                            SortOrder = sortOrder++,
                            MainImage = false
                        });
                    }
                }

                // ✅ Đảm bảo có 1 ảnh chính
                if (!existing.FoodImages.Any(i => i.MainImage) && existing.FoodImages.Any())
                    existing.FoodImages.First().MainImage = true;

                var result = await _menuService.Update(existing);
                if (!result.IsSuccess)
                    return BadRequest(result);

                return Ok(new StatusDTO
                {
                    IsSuccess = true,
                    Message = "✅ Cập nhật món ăn thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new StatusDTO
                {
                    IsSuccess = false,
                    Message = $"Lỗi server: {ex.Message}"
                });
            }
        }

        // =========================
        // DELETE: api/MenuApi/{id}
        // =========================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _menuService.Delete(id);
            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result);
        }

        // =========================
        // GET: api/MenuApi/GetAvailableMenusByCategory
        // (Dành cho User xem menu khả dụng)
        // =========================
        [HttpGet("GetAvailableMenusByCategory")]
        [AllowAnonymous] // hoặc [Authorize(Roles = "User")] nếu muốn bảo vệ
        public async Task<IActionResult> GetAvailableMenusByCategory([FromQuery] int? categoryId = null)
        {
            var result = await _menuService.GetAvailableMenusByCategoryAsync(categoryId);
            if (result == null || !result.Any())
                return NotFound(new { message = "Không có món ăn khả dụng." });

            return Ok(result);
        }
    }
}
