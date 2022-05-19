using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAR_SPARE_PARTS.Models.Store
{
    public class Cart
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int ID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }

        public Cart()
        {
        }
    }
}
