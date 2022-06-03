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
using System.Text.RegularExpressions;

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
            sortComboBox.SelectedIndex = 0;
            if (avp.ProductsList.Count <= 0)
            {
                captionProductItems.Visibility = Visibility.Visible;
            }
            else
            {
                captionProductItems.Visibility = Visibility.Hidden;
            }
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch { }
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
            if (avp.ProductsList.Count <= 0)
            {
                captionProductItems.Visibility = Visibility.Visible;
            }
            else
            {
                captionProductItems.Visibility = Visibility.Hidden;
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
            catch
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

        private void DoOrder(object sender, RoutedEventArgs e)
        {
            if (avp.IsCartEmpty())
            {
                OrderWindow ow = new OrderWindow(UserID);
                ow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Ваша корзина пуста. Добавьте что-нибудь в корзину, чтобы сделать заказ.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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

        private (string, string, int, string) ParcingText(string text)
        {
            Regex titleReg = new Regex(@"[а-я]+");
            Regex quantityReg = new Regex(@"[\d]+\-[\d]+");
            Regex priceReg = new Regex(@"[\d]+byn");
            Regex carBrandReg = new Regex(@"[a-z]+");
            string productSearchingTitle = "";
            string productSearchingQuantity = "";
            string productSearchingPrice = "";
            string productSearchingCarBrand = "";
            int price = 0;
            foreach (Match match in titleReg.Matches(text.ToLower()))
            {
                productSearchingTitle += match + " ";
            }
            foreach (Match match in quantityReg.Matches(text.ToLower()))
            {
                productSearchingQuantity += match + " ";
            }
            foreach (Match match in priceReg.Matches(text.ToLower()))
            {
                productSearchingPrice += match + " ";
            }

            foreach (Match match in carBrandReg.Matches(text.ToLower()))
            {
                if (match.ToString().Length >= 4 && match.ToString() != "euro")
                    productSearchingCarBrand += match + " ";
            }

            if (productSearchingPrice.Length != 0)
            {
                price = Convert.ToInt32(productSearchingPrice.Substring(0, productSearchingPrice.Length - 4));
            }

            return (productSearchingTitle.Trim(), productSearchingQuantity.Trim(), price, productSearchingCarBrand.Trim());
        }

        private List<ProductView> CollectProductsByFilters(string title, int minQuantity, int maxQuantity, int price, string carBrand, string fullQuery)
        {
            List<ProductView> products = new List<ProductView>(avp.GetAllProducts());
            List<ProductView> newProducts = new List<ProductView>();
            List<ProductView> templateList = new List<ProductView>();
            if (title.Length > 0)
            {
                foreach(ProductView p in products.Where(p => p.Title.ToLower().IndexOf(title.ToLower()) != -1))
                {
                    newProducts.Add(p);   
                }
            }
            if (minQuantity > 0 && maxQuantity > 0)
            {
                
                if (newProducts.Count > 0)
                {
                    templateList.Clear();
                    templateList = new List<ProductView>(newProducts);
                    newProducts.Clear();
                    foreach (ProductView p in templateList.Where(p => p.Quantity >= minQuantity && p.Quantity <= maxQuantity))
                    {
                        newProducts.Add(p);
                    }
                }
                else
                {
                    foreach (ProductView p in products.Where(p => p.Quantity >= minQuantity && p.Quantity <= maxQuantity))
                    {
                        newProducts.Add(p);
                    }
                }
            }
            if (price > 0)
            {
                if (newProducts.Count > 0)
                {
                    templateList.Clear();
                    templateList = new List<ProductView>(newProducts);
                    newProducts.Clear();
                    foreach (ProductView p in templateList.Where(p => p.Price <= price))
                    {
                        newProducts.Add(p);
                    }
                }
                else
                {
                    foreach (ProductView p in products.Where(p => p.Price <= price))
                    {
                        newProducts.Add(p);
                    }
                }
            }

            if (carBrand.Length > 0)
            {
                if (newProducts.Count > 0)
                {
                    templateList.Clear();
                    templateList = new List<ProductView>(newProducts);
                    newProducts.Clear();
                    foreach (ProductView p in templateList.Where(p => p.CarBrand.ToLower().IndexOf(carBrand.ToLower()) != -1))
                    {
                        newProducts.Add(p);
                    }
                }
                else
                {

                    foreach (ProductView p in products.Where(p => p.CarBrand.ToLower().IndexOf(carBrand.ToLower()) != -1))
                    {
                        newProducts.Add(p);
                    }
                }
            }
            //if (fullQuery.Length > 0)
            //{
            //    if (newProducts.Count > 0)
            //    {
            //        templateList.Clear();
            //        templateList = new List<ProductView>(newProducts);
            //        if (templateList.Where(p => p.Title.ToLower().IndexOf(fullQuery) != -1).Count() > 0)
            //        {
            //            newProducts.Clear();

            //            foreach (ProductView p in templateList.Where(p => p.Title.ToLower().IndexOf(fullQuery) != -1))
            //            {
            //                newProducts.Add(p);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        foreach (ProductView p in products.Where(p => p.Title.ToLower().IndexOf(fullQuery) != -1))
            //        {
            //            newProducts.Add(p);
            //        }
            //    }
            //}

            return newProducts;
        }

        private void searchProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text == "Поиск")
            {
                avp.ProductsList.Clear();
                foreach (ProductView p in avp.GetAllProducts())
                {
                    avp.ProductsList.Add(p);
                }
                sortComboBox.SelectedIndex = 0;
            }
            else
            {
                (string, string, int, string) summaryQuery = ParcingText(searchTextBox.Text);
                string title = summaryQuery.Item1;
                string quantityInterval = summaryQuery.Item2;
                int minQuantity = quantityInterval.Length > 0 ? Convert.ToInt32(quantityInterval.Split('-')[0]) : 0;
                int maxQuantity = quantityInterval.Length > 0 ? Convert.ToInt32(quantityInterval.Split('-')[0]) : 0;
                int price = summaryQuery.Item3;
                string carBrand = summaryQuery.Item4;
                avp.ProductsList.Clear();
                foreach (ProductView pr in CollectProductsByFilters(title, minQuantity, maxQuantity, price, carBrand, searchTextBox.Text))
                {
                    avp.ProductsList.Add(pr);
                }
                
            }
            if (avp.ProductsList.Count <= 0)
            {
                captionProductItems.Visibility = Visibility.Visible;
            }
            else
            {
                captionProductItems.Visibility = Visibility.Hidden;
            }
            productsListBox.Items.Refresh();
        }
        private void addProductToCart_Click(object sender, RoutedEventArgs e)
        {
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

        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            searchTextBox.Clear();
            searchTextBox.Foreground = Brushes.Black;
        }

        private void searchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text.Length < 1)
            {
                searchTextBox.Text = "Поиск";
            }
            searchTextBox.Foreground = Brushes.Gray;
        }

        private void UpdateListBox(List<ProductView> orderedList)
        {
            avp.ProductsList.Clear();
            foreach (ProductView product in orderedList)
            {
                avp.ProductsList.Add(product);
            }
            productsListBox.Items.Refresh();
        }

        private void ComboBoxItem1_Selected(object sender, RoutedEventArgs e)
        {

            List<ProductView> orderedList = new List<ProductView>(avp.ProductsList.OrderBy(p => p.Title));
            UpdateListBox(orderedList);
        }
        private void ComboBoxItem2_Selected(object sender, RoutedEventArgs e)
        {
            List<ProductView> orderedList = new List<ProductView>(avp.ProductsList.OrderByDescending(p => p.Title));
            UpdateListBox(orderedList);

        }
        private void ComboBoxItem3_Selected(object sender, RoutedEventArgs e)
        {
            List<ProductView> orderedList = new List<ProductView>(avp.ProductsList.OrderBy(p => p.CarBrand));
            UpdateListBox(orderedList);


        }
        private void ComboBoxItem4_Selected(object sender, RoutedEventArgs e)
        {
            List<ProductView> orderedList = new List<ProductView>(avp.ProductsList.OrderByDescending(p => p.CarBrand));
            UpdateListBox(orderedList);


        }
        private void ComboBoxItem5_Selected(object sender, RoutedEventArgs e)
        {
            List<ProductView> orderedList = new List<ProductView>(avp.ProductsList.OrderBy(p => p.Date));
            UpdateListBox(orderedList);
        }
        private void ComboBoxItem6_Selected(object sender, RoutedEventArgs e)
        {
            List<ProductView> orderedList = new List<ProductView>(avp.ProductsList.OrderByDescending(p => p.Date));
            UpdateListBox(orderedList);
        }
        private void ComboBoxItem7_Selected(object sender, RoutedEventArgs e)
        {
            List<ProductView> orderedList = new List<ProductView>(avp.ProductsList.OrderBy(p => p.Price));
            UpdateListBox(orderedList);
        }
        private void ComboBoxItem8_Selected(object sender, RoutedEventArgs e)
        {
            List<ProductView> orderedList = new List<ProductView>(avp.ProductsList.OrderByDescending(p => p.Price));
            UpdateListBox(orderedList);
        }
        private void ComboBoxItem9_Selected(object sender, RoutedEventArgs e)
        {
            List<ProductView> orderedList = new List<ProductView>(avp.ProductsList.OrderBy(p => p.Quantity));
            UpdateListBox(orderedList);
        }
        private void ComboBoxItem10_Selected(object sender, RoutedEventArgs e)
        {
            List<ProductView> orderedList = new List<ProductView>(avp.ProductsList.OrderByDescending(p => p.Quantity));
            UpdateListBox(orderedList);

        }
        private void ComboBoxItem11_Selected(object sender, RoutedEventArgs e)
        {
            List<ProductView> orderedList = new List<ProductView>(avp.ProductsList.OrderByDescending(p => p.Type));
            UpdateListBox(orderedList);

        }
        private void ComboBoxItem12_Selected(object sender, RoutedEventArgs e)
        {
            List<ProductView> orderedList = new List<ProductView>(avp.ProductsList.OrderBy(p => p.Type));
            UpdateListBox(orderedList);
        }

    }
}

