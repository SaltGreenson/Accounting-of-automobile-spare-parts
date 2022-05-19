using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAR_SPARE_PARTS.Models.User
{
    public class UserContext : DbContext
    {

        public UserContext() : base("DefaultConnection")
        {

        }
        public DbSet<User> Users { get; set; }
        
    }

    
}
