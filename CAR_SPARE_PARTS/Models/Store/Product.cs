using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CAR_SPARE_PARTS.Classes;

namespace CAR_SPARE_PARTS.Models.Store
{
    public class Product
    {
        [Key][DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int ID { get; set; }
        [MaxLength(100)] [Required] public string Title { get; set; }
        [Required]  public int CarBrandID { get; set; }


        [Required] public bool Type { get; set; }


        [Required] public string DateOfManufacture { get; set; }
        [Required] public int Quantity { get; set; }
        [Required] public double PricePerPiece { get; set; }

        public Product() { }

        public Product(string title, int carBrandID, bool type, string dateOfManufacture, int quantity, double pricePerPiece)
        {
            Title = title;
            CarBrandID = carBrandID;
            Type = type;
            DateOfManufacture = dateOfManufacture;
            Quantity = quantity;
            PricePerPiece = pricePerPiece;
        }

        public static implicit operator Product(RelayCommand v)
        {
            throw new NotImplementedException();
        }
    }
}
