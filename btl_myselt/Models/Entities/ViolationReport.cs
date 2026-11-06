using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace btl_myselt.Models.Entities
{
    public class ViolationReport
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Người báo cáo")]
        public int ReporterId { get; set; }

        [ForeignKey("ReporterId")]
        public virtual User? Reporter { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Loại đối tượng vi phạm")]
        public string TargetType { get; set; } = "Vehicle"; // "Vehicle", "User", "Booking"

        [Display(Name = "ID đối tượng vi phạm")]
        public int TargetId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hoặc nhập lý do")]
        [StringLength(150)]
        [Display(Name = "Lý do báo cáo")]
        public string Reason { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Mô tả chi tiết")]
        public string? Description { get; set; }

        [StringLength(50)]
        [Display(Name = "Trạng thái xử lý")]
        public string Status { get; set; } = "Pending"; // "Pending", "Resolved", "Dismissed"

        [StringLength(500)]
        [Display(Name = "Ghi chú của Admin")]
        public string? AdminNote { get; set; }
    }
}