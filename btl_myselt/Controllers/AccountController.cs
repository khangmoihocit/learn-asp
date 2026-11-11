using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using btl_myselt.Data;
using btl_myselt.Helpers;
using btl_myselt.Models.Entities;
using btl_myselt.Models.Enums;
using btl_myselt.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace btl_myselt.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                return Json(new { success = false, message = firstError ?? "Dữ liệu không hợp lệ!" });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.Trim().ToLower());

            if (user == null || !PasswordHelper.VerifyPassword(model.Password, user.Password))
            {
                return Json(new { success = false, message = "Email hoặc mật khẩu không chính xác!" });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            //dieu hướng
            string redirectUrl = "/";
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                redirectUrl = model.ReturnUrl;
            }
            else if (user.Role == "Admin")
            {
                redirectUrl = "/Admin/Vehicle";
            }

            return Json(new { success = true, redirectUrl });
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.Communities = await _context.Communities.Where(c => c.IsActive).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                return Json(new { success = false, message = firstError ?? "Dữ liệu không hợp lệ!" });
            }

            bool exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.Trim().ToLower());
            if (exists)
            {
                return Json(new { success = false, message = "Email này đã được đăng ký trong hệ thống!" });
            }

            var user = new User
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim().ToLower(),
                PhoneNumber = model.PhoneNumber.Trim(),
                Password = PasswordHelper.HashPassword(model.Password),
                Role = "USER",
                MemberStatus = MemberStatus.Active,
                CommunityId = model.CommunityId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = "Đăng ký tài khoản thành công! Đang chuyển đến trang đăng nhập...",
                redirectUrl = Url.Action("Login", "Account")
            });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
