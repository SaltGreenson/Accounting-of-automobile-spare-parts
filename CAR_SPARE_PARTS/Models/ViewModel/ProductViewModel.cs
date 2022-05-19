using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CAR_SPARE_PARTS.Models.Store;
namespace CAR_SPARE_PARTS.Models.ViewModel
{
    public class ProductViewModel : INotifyPropertyChanged
    {
        private Product product;

        public ProductViewModel(Product p)
        {
            product = p;
        }

        public string Title
        {
            get { return product.Title; }
            set
            {
                product.Title = value;
                OnPropertyChanged("Title");
            }
        }


        public int CarBrandID
        {
            get 
            { 
                return product.CarBrandID;
            }
            set
            {
                    product.CarBrandID = value;
                    OnPropertyChanged("CarBrandID");
            }
        }

        public bool Type
        {
            get { return product.Type; }
            set
            {
                product.Type = value;
                OnPropertyChanged("Type");
            }
        }

        public string DateOfManufacture
        {
            get { return product.DateOfManufacture; }
            set
            {
                product.DateOfManufacture = value;
                OnPropertyChanged("DateOfManufacture");
            }
        }

        public int Quantity
        {
            get { return product.Quantity; }
            set
            {
                product.Quantity = value;
                OnPropertyChanged("Quantity");
            }
        }

        public double PricePerPiece
        {
            get { return product.PricePerPiece; }
            set
            {
                product.PricePerPiece = value;
                OnPropertyChanged("PricePerPiece");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
