using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using btl_myselt.Models.Enums;

namespace btl_myselt.Models.Entities
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Phương tiện")]
        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public virtual Vehicle? Vehicle { get; set; }

        [Display(Name = "Người thuê")]
        public int BorrowerId { get; set; }

        [ForeignKey("BorrowerId")]
        public virtual User? Borrower { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian bắt đầu")]
        [Display(Name = "Thời gian bắt đầu")]
        public DateTime StartAt { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian kết thúc")]
        [Display(Name = "Thời gian kết thúc")]
        public DateTime EndAt { get; set; }

        [Range(0, 100000000, ErrorMessage = "Mức phí sử dụng không được âm")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tổng phí ước tính (VNĐ)")]
        public decimal EstimatedFee { get; set; } = 0;

        [Display(Name = "Trạng thái đơn")]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Display(Name = "Ngày tạo yêu cầu")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}