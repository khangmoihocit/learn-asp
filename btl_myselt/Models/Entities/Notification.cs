using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace btl_myselt.Models.Entities
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        // Người nhận thông báo
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Nội dung thông báo")]
        public string Content { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Loại thông báo")]
        public string Type { get; set; } = "Booking"; // "Booking", "System", "Violation"

        [Display(Name = "Đã đọc")]
        public bool IsRead { get; set; } = false;

        [Display(Name = "Thời gian gửi")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}