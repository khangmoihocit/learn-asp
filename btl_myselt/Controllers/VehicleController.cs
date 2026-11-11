using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using btl_myselt.Data;
using btl_myselt.Models.Entities;
using btl_myselt.Models.Enums;

namespace btl_myselt.Controllers
{
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public VehicleController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Query lọc xe dùng chung cho cả trang chính và AJAX
        private IQueryable<Vehicle> BuildFilterQuery(string? keyword, int? categoryId, int? communityId, ShareType? shareType, string? sortBy)
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
                                         (v.Description != null && v.Description.ToLower().Contains(kw)) ||
                                         (v.Community != null && v.Community.Name.ToLower().Contains(kw)));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(v => v.CategoryId == categoryId.Value);
            }

            if (communityId.HasValue && communityId.Value > 0)
            {
                query = query.Where(v => v.CommunityId == communityId.Value);
            }

            if (shareType.HasValue)
            {
                query = query.Where(v => v.ShareType == shareType.Value);
            }

            query = sortBy switch
            {
                "price_asc" => query.OrderBy(v => v.Price),
                "price_desc" => query.OrderByDescending(v => v.Price),
                "name" => query.OrderBy(v => v.Name),
                _ => query.OrderByDescending(v => v.Id)
            };

            return query;
        }

        // Trang danh sách xe chính
        public async Task<IActionResult> Index(string? keyword, int? categoryId, int? communityId, ShareType? shareType, string? sortBy)
        {
            var query = BuildFilterQuery(keyword, categoryId, communityId, shareType, sortBy);
            var vehicles = await query.ToListAsync();

            ViewBag.Keyword = keyword;
            ViewBag.SelectedCategory = categoryId ?? 0;
            ViewBag.SelectedCommunity = communityId ?? 0;
            ViewBag.SelectedShareType = shareType;
            ViewBag.SelectedSort = sortBy ?? "";

            ViewBag.Categories = await _context.VehicleCategories.ToListAsync();
            ViewBag.Communities = await _context.Communities.Where(c => c.IsActive).ToListAsync();

            return View(vehicles);
        }

        // AJAX: Lọc và trả về danh sách thẻ xe
        [HttpGet]
        public async Task<IActionResult> GetGrid(string? keyword, int? categoryId, int? communityId, ShareType? shareType, string? sortBy)
        {
            var query = BuildFilterQuery(keyword, categoryId, communityId, shareType, sortBy);
            var vehicles = await query.ToListAsync();
            return PartialView("_VehicleGrid", vehicles);
        }

        // AJAX: Lấy thông tin cho modal xem nhanh
        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
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

        // Trang chi tiết xe đầy đủ (thông số, đánh giá, form đặt thuê xe)
        public async Task<IActionResult> Details(int id)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.Category)
                .Include(v => v.Community)
                .Include(v => v.Owner)
                .Include(v => v.Images)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null) return NotFound();

            // Lấy danh sách đánh giá từ các đơn thuê đã hoàn thành của xe
            var reviews = await _context.Reviews
                .Include(r => r.Reviewer)
                .Where(r => r.Booking != null && r.Booking.VehicleId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.Reviews = reviews;
            ViewBag.AvgRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 1) : 5.0;

            return View(vehicle);
        }

        // Xử lý gửi yêu cầu thuê xe (Yêu cầu đăng nhập: Role USER hoặc Admin)
        [HttpPost]
        public async Task<IActionResult> Book(int vehicleId, DateTime startAt, DateTime endAt)
        {
            // Kiểm tra đăng nhập
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, requireLogin = true, message = "Vui lòng đăng nhập để thực hiện đặt thuê xe!" });
            }

            if (startAt >= endAt)
            {
                return Json(new { success = false, message = "Thời gian trả xe phải sau thời gian nhận xe!" });
            }

            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null || vehicle.Status != VehicleStatus.Available)
            {
                return Json(new { success = false, message = "Phương tiện không khả dụng để thuê!" });
            }

            // Lấy ID người dùng từ Claims đăng nhập
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int borrowerId))
            {
                return Json(new { success = false, requireLogin = true, message = "Phiên đăng nhập không hợp lệ, vui lòng đăng nhập lại!" });
            }

            // Không cho phép tự thuê xe của chính mình
            if (vehicle.OwnerId == borrowerId)
            {
                return Json(new { success = false, message = "Bạn không thể đặt thuê phương tiện do chính mình sở hữu!" });
            }

            // Tính số ngày và phí thuê ước tính
            var days = (int)Math.Ceiling((endAt - startAt).TotalDays);
            if (days < 1) days = 1;

            decimal fee = vehicle.ShareType == ShareType.Free ? 0 : days * vehicle.Price;

            var booking = new Booking
            {
                VehicleId = vehicleId,
                BorrowerId = borrowerId,
                StartAt = startAt,
                EndAt = endAt,
                EstimatedFee = fee,
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.Now
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = $"Gửi yêu cầu thuê xe thành công! Mã đơn: #{booking.Id}.",
                bookingId = booking.Id
            });
        }

        // ================= GỬI YÊU CẦU CHO THUÊ XE =================
        [HttpGet]
        public async Task<IActionResult> RegisterShare()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Vehicle/RegisterShare" });
            }

            ViewBag.Categories = await _context.VehicleCategories.ToListAsync();
            ViewBag.Communities = await _context.Communities.Where(c => c.IsActive).ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterShare(Vehicle vehicle, IFormFile? imageFile)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập trước khi gửi xe!" });
            }

            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int ownerId))
            {
                return Json(new { success = false, message = "Phiên đăng nhập không hợp lệ!" });
            }

            if (string.IsNullOrWhiteSpace(vehicle.Name))
            {
                return Json(new { success = false, message = "Vui lòng nhập tên phương tiện!" });
            }

            vehicle.OwnerId = ownerId;
            vehicle.Status = VehicleStatus.Hidden; // Tạm ẩn, chờ Admin phê duyệt

            if (vehicle.ShareType == ShareType.Free)
            {
                vehicle.Price = 0;
            }

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            // Lưu ảnh nếu người dùng có tải lên
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

                var vehicleImg = new VehicleImage
                {
                    VehicleId = vehicle.Id,
                    ImageUrl = "/uploads/vehicles/" + uniqueFileName,
                    IsPrimary = true
                };
                _context.VehicleImages.Add(vehicleImg);
                await _context.SaveChangesAsync();
            }

            return Json(new
            {
                success = true,
                message = "Gửi yêu cầu đăng xe thành công! Phương tiện đang ở trạng thái chờ Admin phê duyệt trước khi hiển thị công khai.",
                redirectUrl = "/Vehicle"
            });
        }
    }
}
