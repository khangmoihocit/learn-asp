using System.ComponentModel.DataAnnotations;

namespace btl_myselt.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập email!")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Email không đúng định dạng (Ví dụ: name@domain.com)!")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên!")]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }
}