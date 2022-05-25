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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CAR_SPARE_PARTS
{
    public partial class StoreWindow : Window
    {
        AppViewProduct avp;
        private bool IsAdmin { get; set; }
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

        private void FillListBox()
        {
           
        }

        public StoreWindow(bool isAdmin)
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            IsAdmin = isAdmin;
            if (isAdmin)
            {
                SwitchTheme(new Uri("./Styles/StylesForAdmin.xaml", UriKind.Relative));
            }
            else
            {
                SwitchTheme(new Uri("./Styles/StylesForUser.xaml", UriKind.Relative));
            }
            //FillListBox();
            avp = new AppViewProduct();
            DataContext = avp;
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
            if (IsAdmin)
            {
            }
            //string str = productsListBox.SelectedValue.ToString();
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

        private void AddProduct_click(object sender, RoutedEventArgs e)
        {
            avp.AddProduct();
            productsListBox.Items.Refresh();
        }

        private void deleteItemButton_Click(object sender, RoutedEventArgs e)
        {
            //ProductView product = productsListBox.SelectedItem as ProductView;
            //MessageBox.Show(product.Title);
        }

        private void editItemButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
