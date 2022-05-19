using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CAR_SPARE_PARTS.Models.Store;

namespace CAR_SPARE_PARTS.Models.ViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        private Product selectedProduct;
        public ObservableCollection<Product> ProductsObsCollection { get; set; }

        // команда добавления нового объекта
        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                    (addCommand = new RelayCommand(obj =>
                    {
                        using (var dbContext = new ProductContext())
                        {
                            dbContext.Products.Add(new Product());
                            dbContext.SaveChanges();
                        }
                    }));
            }
        }

        // команда удаления
        private RelayCommand removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return removeCommand ??
                  (removeCommand = new RelayCommand(obj =>
                  {
                      Product product = obj as Product;
                      if (product != null)
                      {
                          ProductsObsCollection.Remove(product);
                          using (var dbContext = new ProductContext())
                          {
                              dbContext.Products.Remove(product);
                              dbContext.SaveChanges();
                          }
                      }
                  },
                 (obj) => ProductsObsCollection.Count > 0));
            }
        }

        public Product SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                selectedProduct = value;
                OnPropertyChanged("SelectedPhone");
            }
        }

        public ViewModel()
        {
            using (var dbContext = new ProductContext())
            {
                IQueryable<Product> products = dbContext.Products.OrderBy(p => p.Title);
                ProductsObsCollection = new ObservableCollection<Product>();
                foreach (Product product in products)
                {
                    ProductsObsCollection.Add(product);
                }
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
