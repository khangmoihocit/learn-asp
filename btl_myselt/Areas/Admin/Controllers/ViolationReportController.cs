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
    public class ViolationReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ViolationReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách báo cáo vi phạm
        public async Task<IActionResult> Index(string? status)
        {
            var query = _context.ViolationReports
                .Include(r => r.Reporter)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
                ViewBag.SelectedStatus = status;
            }

            var list = await query.OrderByDescending(r => r.Id).ToListAsync();
            return View(list);
        }

        // Cập nhật trạng thái xử lý báo cáo
        [HttpPost]
        public async Task<IActionResult> Resolve(int id, string status, string? adminNote)
        {
            var report = await _context.ViolationReports.FindAsync(id);
            if (report == null) return Json(new { success = false, message = "Không tìm thấy báo cáo!" });

            report.Status = status;
            report.AdminNote = adminNote;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Cập nhật xử lý báo cáo thành công!" });
        }
    }
}
