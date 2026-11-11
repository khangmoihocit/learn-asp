using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using btl_myselt.Data;
using btl_myselt.Models.Enums;

namespace btl_myselt.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "ADMIN")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Số liệu tổng quan (KPI Cards)
            ViewBag.TotalVehicles = await _context.Vehicles.CountAsync();
            ViewBag.AvailableVehicles = await _context.Vehicles.CountAsync(v => v.Status == VehicleStatus.Available);
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalBookings = await _context.Bookings.CountAsync();
            ViewBag.TotalRevenue = await _context.Bookings
                .Where(b => b.Status == BookingStatus.Completed || b.Status == BookingStatus.Ongoing)
                .SumAsync(b => b.EstimatedFee);

            ViewBag.PendingBookings = await _context.Bookings.CountAsync(b => b.Status == BookingStatus.Pending);
            ViewBag.PendingVehicles = await _context.Vehicles.CountAsync(v => v.Status == VehicleStatus.Hidden);
            ViewBag.PendingReports = await _context.ViolationReports.CountAsync(r => r.Status == "Pending");

            //Thống kê theo Loại xe
            var categoryStats = await _context.Vehicles
                .GroupBy(v => v.Category != null ? v.Category.Name : "Khác")
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
            ViewBag.CategoryNames = categoryStats.Select(c => c.Name).ToList();
            ViewBag.CategoryCounts = categoryStats.Select(c => c.Count).ToList();

            //Thống kê theo Khu dân cư
            var communityStats = await _context.Vehicles
                .GroupBy(v => v.Community != null ? v.Community.Name : "Khác")
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
            ViewBag.CommunityNames = communityStats.Select(c => c.Name).ToList();
            ViewBag.CommunityCounts = communityStats.Select(c => c.Count).ToList();

            //Doanh thu 6 tháng gần nhất
            var sixMonthsAgo = DateTime.Now.AddMonths(-5);
            var monthlyBookings = await _context.Bookings
                .Where(b => b.CreatedAt >= new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1) &&
                            (b.Status == BookingStatus.Completed || b.Status == BookingStatus.Ongoing))
                .ToListAsync();

            var monthlyLabels = new List<string>();
            var monthlyRevenues = new List<decimal>();

            for (int i = 5; i >= 0; i--)
            {
                var monthDate = DateTime.Now.AddMonths(-i);
                var label = $"Tháng {monthDate.Month}/{monthDate.Year}";
                var revenue = monthlyBookings
                    .Where(b => b.CreatedAt.Month == monthDate.Month && b.CreatedAt.Year == monthDate.Year)
                    .Sum(b => b.EstimatedFee);

                monthlyLabels.Add(label);
                monthlyRevenues.Add(revenue);
            }

            ViewBag.MonthlyLabels = monthlyLabels;
            ViewBag.MonthlyRevenues = monthlyRevenues;

            //Danh sách đơn thuê xe mới nhất
            ViewBag.RecentBookings = await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.Borrower)
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View();
        }
    }
}
