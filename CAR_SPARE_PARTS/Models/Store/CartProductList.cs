using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAR_SPARE_PARTS.Models.Store
{
    public class CartProductList
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int ID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }

    }
}
