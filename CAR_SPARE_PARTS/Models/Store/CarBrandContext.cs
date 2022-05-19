using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAR_SPARE_PARTS.Models.Store
{
    public class CarBrandContext : DbContext
    {
        public CarBrandContext() : base("DefaultConnection")
        {

        }
        public DbSet<CarBrand> CarBrands { get; set; }
    }
}

