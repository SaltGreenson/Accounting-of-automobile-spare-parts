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
using CAR_SPARE_PARTS.Pages;

namespace CAR_SPARE_PARTS
{
    public partial class StoreWindow : Window
    {
        AppViewProduct avp;
        private bool IsAdmin { get; set; }
        private int UserID { get; set; }
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
        public StoreWindow(bool isAdmin, int userId)
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            IsAdmin = isAdmin;
            UserID = userId;
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
            avp = new AppViewProduct(userId, false, isAdmin);
            IsEditMode = false;
            DataContext = avp;
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex) { }
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
            }
            else if (IsEditMode)
            {
                addToCartGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                addToCartGrid.Visibility = Visibility.Visible;
                addToCartQuantity.Text = "1";
            }
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
                addToCartGrid.Visibility = Visibility.Hidden;
                SelectedIndex = productsListBox.SelectedIndex;
                confirmItemButton.Visibility = Visibility.Visible;
                editProductBorder.Visibility = Visibility.Visible;
                IsEditMode = true;
            }
            else
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
                avp.SelectedProduct.Date = $"{date.Year}-{date.Month}-{date.Day}";
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
            _frame.Content = new CartPage(UserID);
            searchItemsGrid.Visibility = Visibility.Hidden;
            _frame.Visibility = Visibility.Visible;
            goBackTextBlock.Visibility = Visibility.Visible;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            _frame.Visibility = Visibility.Hidden;
            searchItemsGrid.Visibility = Visibility.Visible;
            goBackTextBlock.Visibility = Visibility.Hidden;
            avp = new AppViewProduct(UserID, false, IsAdmin);
            DataContext = avp;
            productsListBox.Items.Refresh();
        }

        private void addProductToCart_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            int quantity = Convert.ToInt32(addToCartQuantity.Text);
            string title = avp.SelectedProduct.Title;
            quantity = quantity >= 1 ? quantity : 1;
            quantity = quantity <= avp.SelectedProduct.Quantity ? quantity : avp.SelectedProduct.Quantity;
            addToCartQuantity.Text = quantity.ToString();
            avp.AddProductToCart(quantity);
            productsListBox.Items.Refresh();
            MessageBox.Show($"В корзину было добавлено \"{title}\": {quantity}шт.\nЧтобы перейти в корзину нажмите на икноку \"Корзина\"", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            addToCartGrid.Visibility = Visibility.Hidden;
            addToCartQuantity.Text = "";
        }
    }
}
