using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace btl_myselt.Models.Entities
{
    public class VehicleAvailability
    {
        [Key]
        public int Id { get; set; }

        public int VehicleId { get; set; }

        [ForeignKey("VehicleId")]
        public virtual Vehicle? Vehicle { get; set; }

        [Required]
        [Display(Name = "Thời gian bắt đầu rảnh")]
        public DateTime StartAt { get; set; }

        [Required]
        [Display(Name = "Thời gian kết thúc rảnh")]
        public DateTime EndAt { get; set; }
    }
}