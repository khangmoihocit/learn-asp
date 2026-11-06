using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using btl_myselt.Models.Enums;
using Microsoft.Identity.Client;

namespace btl_myselt.Models.Entities
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }

        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public virtual User? Owner { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual VehicleCategory? Category { get; set; }

        public int CommunityId { get; set; }

        [ForeignKey("CommunityId")]
        public virtual Community? Community { get; set; }

        [Required(ErrorMessage = "Tên phương tiện không được để trống")]
        [StringLength(150, ErrorMessage = "Tối đa 150 ký tự")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string? Condition { get; set; } // Ví dụ: "Xe mới 95%, bảo dưỡng định kỳ"
        public ShareType ShareType { get; set; } = ShareType.Free;

        [Range(0, 100000000, ErrorMessage = "Mức phí sử dụng không được âm")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;

        [Display(Name = "Trạng thái xe")]
        public VehicleStatus Status { get; set; } = VehicleStatus.Available;

        public virtual ICollection<VehicleImage> Images { get; set; } = new List<VehicleImage>();
        public virtual ICollection<VehicleAvailability> Availabilities { get; set; } = new List<VehicleAvailability>();
    }
}