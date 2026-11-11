using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using btl_myselt.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace btl_myselt.Areas.Admin.Controllers
{

    [Area("Admin")]
    public class VehicleImageController : Controller
    {
        public ApplicationDbContext _context;

        public VehicleImageController(ApplicationDbContext context){
            _context = context;
        }

        public async Task<IActionResult> Index(){
            var vehicleImages = await _context.VehicleImages.ToListAsync();
            return View(vehicleImages);
        }
    }
}