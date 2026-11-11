using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using btl_myselt.Data;
using btl_myselt.Models.Entities;

namespace btl_myselt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class VehicleAvailabilityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehicleAvailabilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách lịch rảnh xe
        public async Task<IActionResult> Index(int? vehicleId)
        {
            var query = _context.VehicleAvailabilities
                .Include(a => a.Vehicle)
                .AsQueryable();

            if (vehicleId.HasValue && vehicleId.Value > 0)
            {
                query = query.Where(a => a.VehicleId == vehicleId.Value);
                ViewBag.SelectedVehicleId = vehicleId.Value;
            }

            ViewBag.Vehicles = await _context.Vehicles.OrderBy(v => v.Name).ToListAsync();
            var list = await query.OrderByDescending(a => a.StartAt).ToListAsync();
            return View(list);
        }

        // Thêm lịch rảnh xe
        [HttpPost]
        public async Task<IActionResult> Create(int vehicleId, DateTime startAt, DateTime endAt)
        {
            if (startAt >= endAt)
            {
                return Json(new { success = false, message = "Thời gian kết thúc phải sau thời gian bắt đầu!" });
            }

            var availability = new VehicleAvailability
            {
                VehicleId = vehicleId,
                StartAt = startAt,
                EndAt = endAt
            };

            _context.VehicleAvailabilities.Add(availability);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Thêm lịch rảnh xe thành công!" });
        }

        // Xóa lịch rảnh xe
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.VehicleAvailabilities.FindAsync(id);
            if (item == null) return Json(new { success = false, message = "Không tìm thấy lịch rảnh!" });

            _context.VehicleAvailabilities.Remove(item);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Xóa lịch rảnh xe thành công!" });
        }
    }
}
