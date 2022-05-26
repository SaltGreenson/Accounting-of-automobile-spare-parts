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
        private int SelectedIndex { get; set; }
        private bool IsEditMode { get; set; }
        private bool WasWarning { get; set; }
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
        public StoreWindow(bool isAdmin)
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            IsAdmin = isAdmin;
            if (isAdmin)
            {
                SwitchTheme(new Uri("./Styles/StylesForAdmin.xaml", UriKind.Relative));
                deleteItemButton.Visibility = Visibility.Visible;
                addProductButton.Visibility = Visibility.Visible;
                editItemButton.Visibility = Visibility.Visible;
            }
            else
            {
                SwitchTheme(new Uri("./Styles/StylesForUser.xaml", UriKind.Relative));
            }
            avp = new AppViewProduct();
            IsEditMode = false;
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
            if (IsEditMode && productsListBox.SelectedIndex != SelectedIndex)
            {
                productsListBox.SelectedIndex = SelectedIndex;
                MessageBox.Show("Для начала вам нужно сохранить изменения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            if (productsListBox.SelectedItem != null)
            {
                ProductView product = productsListBox.SelectedItem as ProductView;
                MessageBoxResult res = MessageBox.Show($"Вы действительно хотите удалить \"{product.Title}\"?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (res == MessageBoxResult.Yes)
                {
                    avp.DeleteProduct(product);
                    productsListBox.Items.Refresh();
                }  
            }
            else
            {
                MessageBox.Show("Сначала вам нужно выбрать деталь", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void editItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (productsListBox.SelectedItem != null)
            {
                SelectedIndex = productsListBox.SelectedIndex;
                confirmItemButton.Visibility = Visibility.Visible;
                editProductBorder.Visibility = Visibility.Visible;
                IsEditMode = true;
            } else
            {
                MessageBox.Show("Вам необходимо выбрать элемент перед редактированием", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void textChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                
                string text = ((TextBox)sender).Text.Substring(0, ((TextBox)sender).Text.Length - 1);
                if (text != avp.SelectedProduct.Type || text != avp.SelectedProduct.Title || text != avp.SelectedProduct.Price.ToString() || text != avp.SelectedProduct.CarBrand || text != avp.SelectedProduct.Date || text != avp.SelectedProduct.Quantity.ToString())
                {
                    MessageBox.Show("Изменяемое поле не совпадает с текущим выбранным элементом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (!WasWarning)
                        ((TextBox)sender).Text = text;
                    else
                        WasWarning = true;
                }
            } catch (Exception ex)
            {
                MessageBox.Show("Сначала нужно выбрать элемент", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void confirmItemButton_Click(object sender, RoutedEventArgs e)
        {
            confirmItemButton.Visibility = Visibility.Hidden;
            editProductBorder.Visibility = Visibility.Hidden;
            IsEditMode = false;
        }
    }
}
