using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using btl_myselt.Data;
using Microsoft.AspNetCore.Mvc;

namespace btl_myselt.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class VehicleController : Controller
    {
        public ApplicationDbContext _context;

        public VehicleController(ApplicationDbContext context){
            _context = context;
        }

    }
}