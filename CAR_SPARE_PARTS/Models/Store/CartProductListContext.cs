using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAR_SPARE_PARTS.Models.Store
{
    internal class CartProductListContext : DbContext
    {
        public CartProductListContext() : base("DefaultConnection")
        {

        }
        public DbSet<CartProductList> CartProductsList { get; set; }
    }
}
