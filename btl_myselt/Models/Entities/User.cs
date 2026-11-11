using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using btl_myselt.Models.Enums;

namespace btl_myselt.Models.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public int CommunityId { get; set; }

        [ForeignKey("CommunityId")]
        public virtual Community? Community { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(11, ErrorMessage = "Tối đa 11 ký tự")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Mật khẩu không được vượt quá 255 kí tự")]
        [MinLength(6, ErrorMessage = "Mật khẩu cần ít nhất 6 kí tự")]
        public string Password { get; set; } = string.Empty;

        [StringLength(50)]
        public string Role { get; set; } = "USER";

        public MemberStatus MemberStatus { get; set; } = MemberStatus.Pending;
    }
}