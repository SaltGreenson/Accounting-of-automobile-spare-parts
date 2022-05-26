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
        private int UserCartId { get; set; }
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
        public StoreWindow(bool isAdmin, int userCartId)
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            IsAdmin = isAdmin;
            UserCartId = userCartId;
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
            if (IsAdmin && IsEditMode && productsListBox.SelectedIndex != SelectedIndex)
            {
                productsListBox.SelectedIndex = SelectedIndex;
                MessageBox.Show("Для начала вам нужно сохранить изменения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            } else
            {
                addToCartGrid.Visibility = Visibility.Visible;
                addToCartQuantity.Text = "1";
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
                MessageBox.Show("Вам необходимо выбрать деталь перед редактированием", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void confirmItemButton_Click(object sender, RoutedEventArgs e)
        {
            bool fl = false;
            if (avp.SelectedProduct.Quantity <= 0)
            {
                MessageBox.Show("Количество запчастей не может быть равно нулю", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                fl = true;
            }
            if (avp.SelectedProduct.Title.Length <= 2 || avp.SelectedProduct.CarBrand.Length <= 2)
            {   
                MessageBox.Show("Поля должны быть заполнены корректно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                fl = true;
            }
            try
            {
                DateTime date = Convert.ToDateTime(avp.SelectedProduct.Date);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Поле \"Дата\" не может иметь данный формат", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                fl = true;
            }
            if (!fl)
            {
                confirmItemButton.Visibility = Visibility.Hidden;
                editProductBorder.Visibility = Visibility.Hidden;
                IsEditMode = false;
                avp.EditProduct();
                productsListBox.Items.Refresh();
            }
            
        }

        private void GoToCart(object sender, RoutedEventArgs e)
        {
            searchItemsGrid.Visibility = Visibility.Hidden;
            _frame.Visibility = Visibility.Visible;
            goBackTextBlock.Visibility = Visibility.Visible;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            _frame.Visibility = Visibility.Hidden;
            searchItemsGrid.Visibility = Visibility.Visible;
            goBackTextBlock.Visibility = Visibility.Hidden;

        }

        private void addProductToCart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int quantity = Convert.ToInt32(addToCartQuantity.Text);
                quantity = quantity <= avp.SelectedProduct.Quantity? quantity : avp.SelectedProduct.Quantity;
                addToCartQuantity.Text = quantity.ToString();
                if (quantity > 0)
                {

                    MessageBox.Show($"В корзину было добавлено \"{avp.SelectedProduct.Title}\": {quantity}шт.\nЧтобы перейти в корзину нажмите на икноку \"Корзина\"", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show("Неверно введены данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            addToCartGrid.Visibility = Visibility.Hidden;
            addToCartQuantity.Text = "";
        }
    }
}
