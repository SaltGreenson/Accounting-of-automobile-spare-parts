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
        [Required][MaxLength(20)] public string Login { get; set; }
        [Required][MaxLength(20)] public string Password { get; set; }
        [Required] public bool IsAdministrator { get; set; }

        public User(string login, string password, bool isAdministrator)
        {
            Login = login;
            Password = password;
            IsAdministrator = isAdministrator;
        }

        public User() { }
       
    }
}
