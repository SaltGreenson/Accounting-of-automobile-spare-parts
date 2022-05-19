using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CAR_SPARE_PARTS.Models.Store;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAR_SPARE_PARTS.Models.User
{
    public class User
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int ID { get; set; }
        [Required] public string Login { get; set; }
        [Required] public string Password { get; set; }
        [Required] public bool IsAdministrator { get; set; }
        [Required] public int CartID { get; set; }

        public User(string login, string password, bool isAdministrator)
        {
            Login = login;
            Password = password;
            IsAdministrator = isAdministrator;
            Cart cart = new Cart();
            using (var dbContext = new CartContext())
            {
                dbContext.Carts.Add(cart);
                dbContext.SaveChanges();
            }
            CartID = cart.ID;
        }

        public User() { }
       
    }
}
