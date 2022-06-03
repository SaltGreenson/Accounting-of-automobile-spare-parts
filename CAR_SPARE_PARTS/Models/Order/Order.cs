using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAR_SPARE_PARTS.Models.Order
{
    public class Order
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int ID { get; set; }
        [Required] public int UserID { get; set; }
        [Required][MaxLength(25)] public string LastName {get; set;}
        [Required][MaxLength(25)] public string Name { get; set; }
        [Required][MaxLength(25)] public string MiddleName { get; set; }
        [Required][MaxLength(25)] public string Address { get; set; }

        [Required] public string Date { get; set; }
        [Required] public double PriceOfOrder { get; set; }
    }
}
