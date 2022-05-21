using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CAR_SPARE_PARTS.Models.Store;

namespace CAR_SPARE_PARTS.Classes
{
    public class AppViewProduct
    {
        private ProductView selectedProduct;

        public ObservableCollection<ProductView> ProductsObsCollection { get; set; }

        private event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private string GetCarBrand(int id)
        {
            using (var dbContext = new CarBrandContext())
            {
                var car = dbContext.CarBrands.Where(c => c.ID == id);
                return car.Count() > 0 ? car.First().Brand : "Отсутсвует";
            }
        }

        private string GetProductType(bool type) => type ? "Оригинальная запчасть" : "Неоригинальная запчасть";


        private RelayCommand addProductCommand;

        public AppViewProduct()
        {
            ProductsObsCollection = new ObservableCollection<ProductView>();
        }

        public RelayCommand AddProductCommand
        {
            get
            {
                return addProductCommand ??
                    (addProductCommand = new RelayCommand(obj =>
                    {
                        Product pr = new Product
                        {
                            Title = "Новая деталь",
                            Type = false,
                            CarBrandID = 0,
                            DateOfManufacture = "1970-01-01",
                            PricePerPiece = 0,
                            Quantity = 0
                        };

                        ProductView prView = new ProductView
                        {
                            Title = pr.Title,
                            CarBrand = GetCarBrand(pr.CarBrandID),
                            Price = Math.Round(pr.PricePerPiece, 2),
                            Date = pr.DateOfManufacture,
                            Type = GetProductType(pr.Type),
                            Quantity = pr.Quantity
                        };

                        ProductsObsCollection.Insert(0, prView);
                        using (var dbContext = new ProductContext())
                        {
                            dbContext.Products.Add(pr);
                            dbContext.SaveChanges();
                        }
                        SelectedProduct = prView;
                    }));
            }
        }

        public ProductView SelectedProduct
        {
            get => selectedProduct;
            set
            {
                selectedProduct = value;
                OnPropertyChanged("SelectedProduct");
            }
        }
    }
}
