using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using btl_myselt.Data;
using btl_myselt.Models.Entities;
using btl_myselt.Models.Enums;
using Microsoft.AspNetCore.Authorization;

namespace btl_myselt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<VehicleController> _logger;

        public VehicleController(ApplicationDbContext context, IWebHostEnvironment env, ILogger<VehicleController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.Category)
                .Include(v => v.Community)
                .Include(v => v.Owner)
                .Include(v => v.Images)
                .OrderByDescending(v => v.Id)
                .ToListAsync();

            ViewBag.Categories = new SelectList(await _context.VehicleCategories.ToListAsync(), "Id", "Name");
            ViewBag.Communities = new SelectList(await _context.Communities.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
            ViewBag.Owners = new SelectList(await _context.Users.ToListAsync(), "Id", "FullName");

            return View(vehicles);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Vehicle vehicle, IFormFile? imageFile)
        {
            try
            {
                if (vehicle.ShareType == ShareType.Free)
                {
                    vehicle.Price = 0;
                }

                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Dữ liệu nhập không hợp lệ!" });
                }

                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();

                string? imageUrl = null;

                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "vehicles");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    imageUrl = "/uploads/vehicles/" + uniqueFileName;

                    var vehicleImage = new VehicleImage
                    {
                        VehicleId = vehicle.Id,
                        ImageUrl = imageUrl,
                        IsPrimary = true
                    };
                    _context.VehicleImages.Add(vehicleImage);
                    await _context.SaveChangesAsync();
                }

                var categoryName = (await _context.VehicleCategories.FindAsync(vehicle.CategoryId))?.Name;
                var communityName = (await _context.Communities.FindAsync(vehicle.CommunityId))?.Name;
                var ownerName = (await _context.Users.FindAsync(vehicle.OwnerId))?.FullName;

                return Json(new
                {
                    success = true,
                    message = "Thêm phương tiện thành công!",
                    data = new
                    {
                        id = vehicle.Id,
                        name = vehicle.Name,
                        categoryName = categoryName ?? "—",
                        communityName = communityName ?? "—",
                        ownerName = ownerName ?? "—",
                        shareType = vehicle.ShareType.ToString(),
                        price = vehicle.Price,
                        status = vehicle.Status.ToString(),
                        imageUrl = imageUrl ?? "/images/default-vehicle.png"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm phương tiện");
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicle(int id)
        {
            try
            {
                var v = await _context.Vehicles.FindAsync(id);
                if (v == null) return Json(new { success = false, message = "Không tìm thấy xe!" });

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        id = v.Id,
                        name = v.Name,
                        categoryId = v.CategoryId,
                        communityId = v.CommunityId,
                        ownerId = v.OwnerId,
                        price = v.Price,
                        shareType = (int)v.ShareType,
                        status = (int)v.Status,
                        condition = v.Condition ?? string.Empty,
                        description = v.Description ?? string.Empty
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin xe ID: {Id}", id);
                return Json(new { success = false, message = "Không thể lấy thông tin xe!" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleDetails(int id)
        {
            try
            {
                var v = await _context.Vehicles
                    .Include(x => x.Category)
                    .Include(x => x.Community)
                    .Include(x => x.Owner)
                    .Include(x => x.Images)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (v == null) return Json(new { success = false, message = "Không tìm thấy xe!" });

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        id = v.Id,
                        name = v.Name,
                        categoryName = v.Category?.Name ?? "—",
                        communityName = v.Community?.Name ?? "—",
                        communityAddress = v.Community?.Address ?? "—",
                        ownerName = v.Owner?.FullName ?? "—",
                        ownerPhone = v.Owner?.PhoneNumber ?? "—",
                        ownerEmail = v.Owner?.Email ?? "—",
                        price = v.Price,
                        condition = string.IsNullOrEmpty(v.Condition) ? "Bình thường" : v.Condition,
                        description = string.IsNullOrEmpty(v.Description) ? "Không có mô tả chi tiết" : v.Description,
                        shareType = v.ShareType == ShareType.Free ? "Miễn phí" : "Cho thuê",
                        status = v.Status.ToString(),
                        images = v.Images.Select(i => new
                        {
                            id = i.Id,
                            imageUrl = i.ImageUrl,
                            isPrimary = i.IsPrimary
                        }).ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết xe ID: {Id}", id);
                return Json(new { success = false, message = "Không thể lấy thông tin chi tiết xe!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle)
        {
            try
            {
                if (id != vehicle.Id) return Json(new { success = false, message = "Không tìm thấy xe!" });

                if (vehicle.ShareType == ShareType.Free)
                {
                    vehicle.Price = 0;
                }

                if (ModelState.IsValid)
                {
                    _context.Vehicles.Update(vehicle);
                    await _context.SaveChangesAsync();

                    var categoryName = (await _context.VehicleCategories.FindAsync(vehicle.CategoryId))?.Name;
                    var communityName = (await _context.Communities.FindAsync(vehicle.CommunityId))?.Name;
                    var ownerName = (await _context.Users.FindAsync(vehicle.OwnerId))?.FullName;

                    return Json(new
                    {
                        success = true,
                        message = "Cập nhật thành công!",
                        data = new
                        {
                            id = vehicle.Id,
                            name = vehicle.Name,
                            categoryName = categoryName ?? "—",
                            communityName = communityName ?? "—",
                            ownerName = ownerName ?? "—",
                            shareType = vehicle.ShareType.ToString(),
                            price = vehicle.Price,
                            status = vehicle.Status.ToString()
                        }
                    });
                }

                return Json(new { success = false, message = "Dữ liệu cập nhật không hợp lệ!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật xe ID: {Id}", id);
                return Json(new { success = false, message = "Lỗi khi cập nhật: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var vehicle = await _context.Vehicles
                    .Include(v => v.Images)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (vehicle != null)
                {
                    if (vehicle.Images != null && vehicle.Images.Any())
                    {
                        _context.VehicleImages.RemoveRange(vehicle.Images);
                    }

                    _context.Vehicles.Remove(vehicle);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Xóa phương tiện thành công!" });
                }

                return Json(new { success = false, message = "Không tìm thấy xe để xóa!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa xe ID: {Id}", id);
                return Json(new { success = false, message = "Không thể xóa phương tiện vì đang có dữ liệu liên quan!" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicleImages(int vehicleId)
        {
            try
            {
                var images = await _context.VehicleImages
                    .Where(img => img.VehicleId == vehicleId)
                    .OrderByDescending(img => img.IsPrimary)
                    .Select(img => new
                    {
                        id = img.Id,
                        imageUrl = img.ImageUrl,
                        isPrimary = img.IsPrimary
                    })
                    .ToListAsync();

                return Json(new { success = true, images = images });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy ảnh xe ID: {VehicleId}", vehicleId);
                return Json(new { success = false, images = new object[0] });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadMoreImage(int vehicleId, IFormFile? imageFile)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn file ảnh hợp lệ!" });
                }

                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "vehicles");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                bool hasImages = await _context.VehicleImages.AnyAsync(i => i.VehicleId == vehicleId);

                var newImg = new VehicleImage
                {
                    VehicleId = vehicleId,
                    ImageUrl = "/uploads/vehicles/" + uniqueFileName,
                    IsPrimary = !hasImages
                };
                _context.VehicleImages.Add(newImg);
                await _context.SaveChangesAsync();

                return Json(new { success = true, image = newImg });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải thêm ảnh cho xe ID: {VehicleId}", vehicleId);
                return Json(new { success = false, message = "Lỗi khi lưu ảnh: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetPrimaryImage(int imageId, int vehicleId)
        {
            try
            {
                var images = await _context.VehicleImages.Where(i => i.VehicleId == vehicleId).ToListAsync();
                foreach (var img in images)
                {
                    img.IsPrimary = (img.Id == imageId);
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đặt ảnh chính ID: {ImageId}", imageId);
                return Json(new { success = false, message = "Lỗi hệ thống!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            try
            {
                var img = await _context.VehicleImages.FindAsync(imageId);
                if (img != null)
                {
                    _context.VehicleImages.Remove(img);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Không tìm thấy ảnh!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa ảnh ID: {ImageId}", imageId);
                return Json(new { success = false, message = "Lỗi khi xóa ảnh!" });
            }
        }

        // Phê duyệt nhanh phương tiện (Chuyển từ Tạm ẩn sang Sẵn sàng)
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return Json(new { success = false, message = "Không tìm thấy xe!" });

            vehicle.Status = VehicleStatus.Available;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Phê duyệt phương tiện thành công! Xe đã sẵn sàng hiển thị trên trang chủ." });
        }
    }
}
