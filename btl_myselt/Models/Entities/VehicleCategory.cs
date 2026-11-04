using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace btl_myselt.Models.Entities
{
    public class VehicleCategory
    {
        [Key]
        public int Id {get; set;}

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        [Display(Name = "Tên loại xe")]
        public string Name {get; set;} = string.Empty;

        [Display(Name = "Mô  tả")]
        public string? Description {get; set;}
    }
}