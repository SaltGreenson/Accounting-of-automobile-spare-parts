using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAR_SPARE_PARTS.Models.Store
{
    public class ProductContext : DbContext
    {
        public ProductContext() : base("DefaultConnection")
        {

        }
        public DbSet<Product> Products { get; set; }
    }
}
