using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using btl_myselt.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace btl_myselt.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
         : base(options)
        {

        }

        public DbSet<VehicleCategory> VehicleCategories {get; set;}
    }
}