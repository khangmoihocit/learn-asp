using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace btl_myselt.Models.Entities
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Đơn thuê")]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public virtual Booking? Booking { get; set; }

        // Người đánh giá
        public int ReviewerId { get; set; }

        [ForeignKey("ReviewerId")]
        public virtual User? Reviewer { get; set; }

        // Người được đánh giá
        public int RevieweeId { get; set; }

        [ForeignKey("RevieweeId")]
        public virtual User? Reviewee { get; set; }

        [Range(1, 5, ErrorMessage = "Điểm đánh giá phải từ 1 đến 5 sao")]
        [Display(Name = "Số sao đánh giá")]
        public int Rating { get; set; } = 5;

        [StringLength(500, ErrorMessage = "Nhận xét tối đa 500 ký tự")]
        [Display(Name = "Nội dung nhận xét")]
        public string? Comment { get; set; }

        [Display(Name = "Thời gian đánh giá")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}