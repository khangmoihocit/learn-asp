using System.ComponentModel.DataAnnotations;

namespace btl_myselt.Models.Enums
{
    public enum VehicleStatus
    {
        [Display(Name = "Sẵn sàng")]
        Available = 0,

        [Display(Name = "Đang thuê")]
        Rented = 1,

        [Display(Name = "Bảo trì")]
        Maintenance = 2,

        [Display(Name = "Tạm ẩn")]
        Hidden = 3
    }
}
