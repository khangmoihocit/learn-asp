using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using btl_myselt.Data;
using btl_myselt.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace btl_myselt.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public VehicleController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
        public async Task<IActionResult> Create(Vehicle vehicle, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
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
                        categoryName = categoryName,
                        communityName = communityName,
                        ownerName = ownerName,
                        price = vehicle.Price,
                        status = vehicle.Status.ToString(),
                        imageUrl = imageUrl ?? "/images/default-vehicle.png"
                    }
                });
            }
            return Json(new { success = false, message = "Dữ liệu không hợp lệ! Vui lòng kiểm tra lại." });
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicle(int id)
        {
            var v = await _context.Vehicles.FindAsync(id);
            if (v == null) return Json(new { success = false });
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
                    condition = v.Condition,
                    description = v.Description,
                    status = (int)v.Status
                }
            });
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle)
        {
            if (id != vehicle.Id) return Json(new { success = false, message = "Không tìm thấy xe!" });
            if (ModelState.IsValid)
            {
                _context.Update(vehicle);
                await _context.SaveChangesAsync();
                var categoryName = (await _context.VehicleCategories.FindAsync(vehicle.CategoryId))?.Name;
                var communityName = (await _context.Communities.FindAsync(vehicle.CommunityId))?.Name;
                return Json(new
                {
                    success = true,
                    message = "Cập nhật thành công!",
                    data = new
                    {
                        id = vehicle.Id,
                        name = vehicle.Name,
                        categoryName = categoryName,
                        communityName = communityName,
                        price = vehicle.Price,
                        status = vehicle.Status.ToString()
                    }
                });
            }
            return Json(new { success = false, message = "Dữ liệu cập nhật không hợp lệ!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa phương tiện thành công!" });
            }
            return Json(new { success = false, message = "Không tìm thấy xe để xóa!" });
        }
    }
}