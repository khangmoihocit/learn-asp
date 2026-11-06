using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace btl_myselt.Models.Entities
{
    public class HandoverRecord
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Đơn thuê xe")]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking? Booking { get; set; }

        [Display(Name = "Thời gian bàn giao xe")]
        public DateTime? HandedOverAt { get; set; }

        [Display(Name = "Thời gian hoàn trả xe")]
        public DateTime? ReturnedAt { get; set; }

        [StringLength(500)]
        [Display(Name = "Tình trạng xe lúc nhận")]
        public string? ConditionBefore { get; set; } // Ví dụ: "Xăng 80%, có vết xước nhẹ gương phải"

        [StringLength(500)]
        [Display(Name = "Tình trạng xe lúc trả")]
        public string? ConditionAfter { get; set; } // Ví dụ: "Xe nguyên vẹn, đã rửa sạch"
    }
}