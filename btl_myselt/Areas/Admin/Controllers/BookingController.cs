using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using btl_myselt.Data;
using btl_myselt.Models.Entities;
using btl_myselt.Models.Enums;

namespace btl_myselt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách đơn thuê xe
        public async Task<IActionResult> Index(BookingStatus? status)
        {
            var query = _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.Borrower)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
                ViewBag.SelectedStatus = status.Value;
            }

            var list = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
            return View(list);
        }

        // Cập nhật trạng thái đơn (Phê duyệt, Từ chối, Hoàn thành, Hủy)
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, BookingStatus status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return Json(new { success = false, message = "Không tìm thấy đơn thuê!" });

            booking.Status = status;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật trạng thái đơn thành công!" });
        }
    }
}
