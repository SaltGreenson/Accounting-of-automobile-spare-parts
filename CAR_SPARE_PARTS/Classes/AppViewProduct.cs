using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CAR_SPARE_PARTS.Models.Store;

namespace CAR_SPARE_PARTS.Classes
{
    public class AppViewProduct
    {
        private ProductView selectedProduct;

        public List<ProductView> ProductsList { get; set; }

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



        public AppViewProduct()
        {
            using (var dbContext = new ProductContext())
            {
                IQueryable<Product> products = dbContext.Products;
                ProductsList = new List<ProductView>();

                foreach (Product product in products)
                {
                    ProductsList.Add(new ProductView
                    {
                        ID = product.ID,
                        Title = product.Title,
                        CarBrand = GetCarBrand(product.CarBrandID),
                        Price = Math.Round(product.PricePerPiece, 2),
                        Date = product.DateOfManufacture,
                        Type = GetProductType(product.Type),
                        Quantity = product.Quantity
                    });

                }
                OnPropertyChanged("ProductsObsCollection");
            }
            
        }

        public void AddProduct()
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
            using (var dbContext = new ProductContext())
            {
                dbContext.Products.Add(pr);
                dbContext.SaveChanges();
            }
            ProductView prView = new ProductView
            {
                ID = pr.ID,
                Title = pr.Title,
                CarBrand = GetCarBrand(pr.CarBrandID),
                Price = Math.Round(pr.PricePerPiece, 2),
                Date = pr.DateOfManufacture,
                Type = GetProductType(pr.Type),
                Quantity = pr.Quantity
            };

            ProductsList.Add(prView);
            SelectedProduct = prView;
        }

        public void DeleteProduct(ProductView product)
        {
            using (var dbContext = new ProductContext())
            {
                dbContext.Products.Remove(dbContext.Products.Where(p => p.ID == product.ID).First());
                ProductsList.Remove(product);
                dbContext.SaveChanges();
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
