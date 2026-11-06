using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace btl_myselt.Models.Entities
{
    public class VehicleImage
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Đường dẫn ảnh không được để trống")]
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự")]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPrimary { get; set; } = false;

        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public virtual Vehicle? Vehiclle { get; set; }
    }
}