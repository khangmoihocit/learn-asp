using System.ComponentModel.DataAnnotations;

namespace btl_myselt.Models.Enums
{
    public enum ShareType
    {
        [Display(Name = "Miễn phí")]
        Free = 0,

        [Display(Name = "Cho thuê")]
        Rental = 1
    }
}
