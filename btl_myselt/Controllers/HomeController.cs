using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using btl_myselt.Data;
using btl_myselt.Models;
using btl_myselt.Models.Entities;
using btl_myselt.Models.Enums;

namespace btl_myselt.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? keyword, int? categoryId, int? communityId)
    {
        var query = _context.Vehicles
            .Include(v => v.Category)
            .Include(v => v.Community)
            .Include(v => v.Owner)
            .Include(v => v.Images)
            .Where(v => v.Status == VehicleStatus.Available);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim().ToLower();
            query = query.Where(v => v.Name.ToLower().Contains(kw) || 
                                     (v.Community != null && v.Community.Name.ToLower().Contains(kw)));
            ViewBag.Keyword = keyword;
        }

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(v => v.CategoryId == categoryId.Value);
            ViewBag.SelectedCategory = categoryId.Value;
        }

        if (communityId.HasValue && communityId.Value > 0)
        {
            query = query.Where(v => v.CommunityId == communityId.Value);
            ViewBag.SelectedCommunity = communityId.Value;
        }

        var vehicles = await query.OrderByDescending(v => v.Id).ToListAsync();

        ViewBag.Categories = await _context.VehicleCategories.ToListAsync();
        ViewBag.Communities = await _context.Communities.Where(c => c.IsActive).ToListAsync();

        return View(vehicles);
    }

    [HttpGet]
    public async Task<IActionResult> GetVehicleDetails(int id)
    {
        var v = await _context.Vehicles
            .Include(x => x.Category)
            .Include(x => x.Community)
            .Include(x => x.Owner)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (v == null) return Json(new { success = false, message = "Không tìm thấy thông tin xe!" });

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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
