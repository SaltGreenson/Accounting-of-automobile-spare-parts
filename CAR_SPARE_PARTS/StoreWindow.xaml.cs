using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CAR_SPARE_PARTS.Models.Store;
using CAR_SPARE_PARTS.Classes;

namespace CAR_SPARE_PARTS
{
    /// <summary>
    /// Interaction logic for StoreWindow.xaml
    /// </summary>
    public partial class StoreWindow : Window
    {
        public StoreWindow()
        {
            InitializeComponent();
        }

        private void SwitchTheme(Uri uri)
        {
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }

        private string GetCarBrand(int id)
        {
            using (var dbContext = new CarBrandContext())
            {
                var car = dbContext.CarBrands.Where(c => c.ID == id);
                return car.Count() > 0? car.First().Brand : "Отсутсвует";
            }
        }

        private string GetProductType(bool type) => type ? "Оригинальная запчасть" : "Неоригинальная запчасть";

        private void FillListBox()
        {
            using (var dbContext = new ProductContext())
            {
                var products = dbContext.Products;
                foreach (Product product in products)
                {
                    productsListBox.Items.Add(new ProductView
                    {
                        Title = product.Title,
                        CarBrand = GetCarBrand(product.CarBrandID),
                        Price = Math.Round(product.PricePerPiece, 2),
                        Date = product.DateOfManufacture,
                        Type = GetProductType(product.Type),
                        Quantity = product.Quantity
                    });
                }
            }
        }

        public StoreWindow(bool isAdmin)
        {
            InitializeComponent();
            this.Closing += Window_Closing;

            if (isAdmin)
            {
                SwitchTheme(new Uri("./Styles/StylesForAdmin.xaml", UriKind.Relative));
            }
            else
            {
                SwitchTheme(new Uri("./Styles/StylesForUser.xaml", UriKind.Relative));
            }
            FillListBox();
           
            //DataContext = new ViewModel();
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            try
            {
                Close();
            } catch (Exception ex) { }
            MainWindow mw = new MainWindow();
            SwitchTheme(new Uri("./Styles/StylesForUser.xaml", UriKind.Relative));
            mw.Show();
            
        }

        private void productsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string str = productsListBox.SelectedValue.ToString();
        }


        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }


    }
}
