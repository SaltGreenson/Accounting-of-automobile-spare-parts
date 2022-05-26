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

        private int FindAndUpdateCarBrand(string carBrand)
        {
            using (var dbContext = new CarBrandContext())
            {
                var cb = dbContext.CarBrands.Where(c => c.Brand == carBrand);
                if (cb.Count() <= 0)
                {
                    dbContext.CarBrands.Add(new CarBrand { Brand = carBrand });
                    dbContext.SaveChanges();
                    return dbContext.CarBrands.Where(c => c.Brand == carBrand).First().ID;
                }
                return cb.First().ID;
            }
        }

        private string GetProductType(bool type) => type ? "Оригинальная запчасть" : "Неоригинальная запчасть";
        private bool GetProductType(string type) => type == "Оригинальная запчасть"? true : false;



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

        public void EditProduct()
        {
            using (var dbContext = new ProductContext())
            {
                Product pr = dbContext.Products.Where(p => p.ID == SelectedProduct.ID).First();
                int carBrandId = FindAndUpdateCarBrand(SelectedProduct.CarBrand);
                pr.CarBrandID = carBrandId;
                pr.Title = SelectedProduct.Title;
                pr.Quantity = SelectedProduct.Quantity;
                pr.DateOfManufacture = SelectedProduct.Date;
                pr.PricePerPiece = SelectedProduct.Price;
                pr.Type = GetProductType(SelectedProduct.Type);
                dbContext.SaveChanges();
                SelectedProduct.Type = GetProductType(pr.Type);
                OnPropertyChanged("SelectedProduct");

            }

        }

        public void AddProductToCart(int quantity, int userCartId)
        {
            using (var dbCartContext = new CartContext())
            {
                Cart cart = dbCartContext.Carts.Where(c => c.ID == userCartId).First();
                using (var dbProductContext = new ProductContext())
                {
                    // нужно добавить таблицу в бд "CartProductList"
                    // добавить создание листа при создании пользователя
                    // тут нужно будет достать id листа продуктов и в нем искать продукты
                    // если продукты будут то увеличивать счетчик, если нет то добавлять новый продукт в "CartProductList"
                }
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
