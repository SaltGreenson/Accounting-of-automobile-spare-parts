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
using CAR_SPARE_PARTS.Models.User;


namespace CAR_SPARE_PARTS.Classes
{
    public class AppViewProduct
    {
        private ProductView selectedProduct;
        private bool IsAdmin {get; set;}
        public List<ProductView> ProductsList { get; set; }
        public List<ProductView> CartProducts { get; set; }
        private int UserID { get; set; } 

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

        private ProductView ConvertProductToProductView(Product product)
        {
            return new ProductView
            {
                ID = product.ID,
                Title = product.Title,
                CarBrand = GetCarBrand(product.CarBrandID),
                Price = Math.Round(product.PricePerPiece, 2),
                Date = product.DateOfManufacture,
                Type = GetProductType(product.Type),
                Quantity = product.Quantity
            };
        }

        private Product GetProductById(int id)
        {
            using (var dbContext = new ProductContext())
            {
                Product pr = dbContext.Products.SingleOrDefault(p => p.ID == id);
                return pr != null ? pr : throw new Exception("Производство данного продукта закончилось. Приносим наши извенения!");
            }
        }

        public List<ProductView> GetAllProducts()
        {
            List<ProductView> list = new List<ProductView>();
            using (var dbContext = new ProductContext())
            {
                IQueryable<Product> products = dbContext.Products;
                foreach (Product product in products)
                {
                    if (IsAdmin)
                    {
                        list.Add(ConvertProductToProductView(product));
                    }
                    else if (product.Quantity >= 1)
                    {
                        list.Add(ConvertProductToProductView(product));
                    }

                }
            }
            return list;
        }

        public bool IsCartEmpty()
        {
            using (var dbContext = new CartProductListContext())
            {
                return dbContext.CartProductsList.Where(p => p.UserID == UserID).Count() > 0;
            }
        }
        public AppViewProduct(int userId)
        {
            UserID = userId;
        }

        public AppViewProduct(int userId, bool isCartPage, bool isAdmin)
        {
            UserID = userId;
            IsAdmin = isAdmin;
            if (!isCartPage)
            {
                using (var dbContext = new ProductContext())
                {
                    IQueryable<Product> products = dbContext.Products;
                    ProductsList = new List<ProductView>();
                    foreach (Product product in products)
                    {
                        if (isAdmin)
                        {
                            ProductsList.Add(ConvertProductToProductView(product));
                        }
                        else if (product.Quantity >= 1)
                        {
                            ProductsList.Add(ConvertProductToProductView(product));
                        }

                    }
                    OnPropertyChanged("ProductsList");
                }
            }
            else
            {
                using (var dbCartProductsContext = new CartProductListContext())
                {
                    IQueryable<CartProductList> productsInCartIdies = dbCartProductsContext.CartProductsList.Where(p => p.UserID == UserID);
                    CartProducts = new List<ProductView>();
                    foreach (CartProductList cartProduct in productsInCartIdies)
                    {
                        Product product = GetProductById(cartProduct.ProductID);
                        ProductView prView = ConvertProductToProductView(product);
                        prView.Quantity = cartProduct.Quantity;
                        CartProducts.Add(prView);
                    }
                }
                OnPropertyChanged("CartProducts");
            }
        }

        public double GetOrderSum()
        {
            double sum = 0;
            using (var dbCartProductsContext = new CartProductListContext())
            {
                IQueryable<CartProductList> productsInCartIdies = dbCartProductsContext.CartProductsList.Where(p => p.UserID == UserID);
                foreach (CartProductList cartProduct in productsInCartIdies)
                {
                    double price = GetProductById(cartProduct.ProductID).PricePerPiece;
                    int quantity = cartProduct.Quantity;
                    sum += price * quantity;
                }
            }
            return sum;
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

        public void AddProductToCart(int quantity)
        {

            using (var dbCartProductListContext = new CartProductListContext())
            {
                var existingProduct = dbCartProductListContext.CartProductsList.SingleOrDefault(p => p.ProductID == SelectedProduct.ID && p.UserID == UserID);
                if (existingProduct != null)
                {
                    existingProduct.Quantity += existingProduct.Quantity + quantity <= SelectedProduct.Quantity ? quantity : SelectedProduct.Quantity;
                }
                else
                {
                    dbCartProductListContext.CartProductsList.Add(new CartProductList
                    {
                        UserID = UserID,
                        ProductID = SelectedProduct.ID,
                        Quantity = quantity
                    });
                }
                dbCartProductListContext.SaveChanges();
            }
            
            using (var dbProductContext = new ProductContext())
            {
                Product pr = dbProductContext.Products.SingleOrDefault(p => p.ID == SelectedProduct.ID);
                ProductView pl = ProductsList.SingleOrDefault(p => p.ID == SelectedProduct.ID);
                if (pr.Quantity - quantity <= 0)
                {
                    pr.Quantity = 0;
                    if (!IsAdmin)
                        ProductsList.Remove(pl);
                    else
                        ProductsList.SingleOrDefault(p => p.ID == SelectedProduct.ID).Quantity = 0;
                }
                else
                {
                    pr.Quantity -= quantity;
                    ProductsList.SingleOrDefault(p => p.ID == SelectedProduct.ID).Quantity -= quantity;
                }
                dbProductContext.SaveChanges();
            }
            OnPropertyChanged("ProductsList");
        }

        public void RemoveFromCart(int quantity)
        {
            using (var dbContext = new CartProductListContext())
            {
                using (var dbProductsContext = new ProductContext())
                {
                    Product pr = dbProductsContext.Products.Where(p => p.ID == SelectedProduct.ID).First();

                    if (SelectedProduct.Quantity - quantity <= 0)
                    {
                        dbContext.CartProductsList.Remove(dbContext.CartProductsList.Where(p => p.ProductID == SelectedProduct.ID).First());
                        CartProducts.Remove(CartProducts.Where(p => p.ID == SelectedProduct.ID).First());
                        pr.Quantity += SelectedProduct.Quantity;

                    }
                    else
                    {
                        dbContext.CartProductsList.Where(p => p.ProductID == SelectedProduct.ID).First().Quantity -= quantity;
                        CartProducts.SingleOrDefault(p => p.ID == SelectedProduct.ID).Quantity -= quantity;
                        pr.Quantity += quantity;
                    }
                    dbProductsContext.SaveChanges();
                }
                dbContext.SaveChanges();
            }
            SelectedProduct = null;
            OnPropertyChanged("CartProducts");
            OnPropertyChanged("SelectedProduct");

            
        }

        public void AddProduct()
        {
            Product pr = new Product
            {
                Title = "Новая деталь",
                Type = false,
                CarBrandID = 0,
                DateOfManufacture = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}",
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
                dbContext.Products.Where(p => p.ID == product.ID).First().Quantity = 0;
                if (!IsAdmin)
                    ProductsList.Remove(product);
                else
                    ProductsList.SingleOrDefault(p => p.ID == product.ID).Quantity = 0;
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
