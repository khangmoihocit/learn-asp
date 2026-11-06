using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace btl_myselt.Models.Entities
{
    public class Community
    {
        public int Id {get; set;}

        [Required(ErrorMessage = "Tên khu dân cư không được để trống")]
        public string Name {get; set;} = string.Empty;
        public string? Address {get; set;}
        public bool IsActive {get; set;} = false;

    }
}