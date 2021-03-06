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
                exportTextBlock.Visibility = Visibility.Visible;
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
            else if (avp.SelectedProduct != null)
            {
                addToCartGrid.Visibility = Visibility.Visible;
                addToCartQuantity.Text = "1";
            } 
            else
            {
                addToCartGrid.Visibility = Visibility.Hidden;
            }
        }

        private void AddProduct_click(object sender, RoutedEventArgs e)
        {
            avp.AddProduct();
            productsListBox.Items.Refresh();
            productsListBox.SelectedIndex = productsListBox.Items.Count - 1;
            editItemButton_Click(sender, e);
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
            if (avp.SelectedProduct != null)
            {
                try
                {
                    avp.SelectedProduct.Quantity = Convert.ToInt32(quantityEditTextBox.Text);
                    avp.SelectedProduct.Price = Convert.ToInt32(priceEditTextBox.Text);
                    addToCartGrid.Visibility = Visibility.Hidden;
                    SelectedIndex = productsListBox.SelectedIndex;
                    confirmItemButton.Visibility = Visibility.Visible;
                    editProductBorder.Visibility = Visibility.Visible;
                    IsEditMode = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Была вызвана ошибка.\nПодробности:\t{ex.Data}\n\t{ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Вам необходимо выбрать деталь перед редактированием", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void confirmItemButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Convert.ToInt32(priceEditTextBox.Text);
                Convert.ToInt32(quantityEditTextBox.Text);

            }
            catch
            {
                MessageBox.Show("Введите корректные данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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

        private void DisplayReference(object sender, RoutedEventArgs e)
        {
            string text = "\tРуководство по использованию\n" +
                "На рабочем экране вы можете наблюдать:\n" +
                "  1. Корзина - предназначена для просмотра и редактирования товаров, ранее добавленных в корзину.\n" +
                "  2. Экспорт данных - выполняет экспорт ранее осуществяемых заказов в MS Word. Эта функция работает только в том случае, если вы являетесь администратором.\n" +
                "  3. Знак \"$\" - означает выполнить заказ. Перейдя на страницу заказа вам необзодимо будет заполнить свои личные данные и время доставки товаров, ранее добавленных в корзину.\n" +
                "  4. Панель поиска - это панель, предназначенная для поиска товаров по заданным вами фильтрам. Поиск может осуществляться по следующим полям:\n" +
                "\tА) По названию.\n" +
                "\tБ) По цене, в конце должно быть указано BYN\n" +
                "\tВ) По марке автомобиля.\n" +
                "\tГ) По количеству, указывается ОТ и ДО.\n" +
                "   А также вы можете комбинировать вышеперечислнные фильтры и выполнять более точный поиск товара. Пример запроса поиска: Подкрылок передний левый Audi 1-30 900BYN. Если по данному запросу не будет найдено товара, то система поиска постарается найти похожие товары по заданному фильтру.\n" +
                "  5. Сортировка товаров - здесь вы можете выбрать подходящую для вас сортировку автомобильных запчастей.\n" +
                "  6. Кнопка \"Удалить\" - удаляет выбранный вами товар." +
                "  7. Кнопка \"Добавить\" - добавляет новую деталь в список товаров. Эта функция работает только в том случае, если вы являетесь администратором.\n" +
                "  8. Кнопка \"Редактировать\" - позволяет редактировать выбранный вами товар. Эта функция работает только в том случае, если вы являетесь администратором.\n" +
                "  9. Кнопка \"Сохранить\" - сохраняет внесенные изменения после редактирования\n" +
                "  10. Список автомобильных запчастей - отображает весь список запчастей которые были добавлены администраторами\n" +
                "\n" +
                "\t%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n" +
                "%%%%%%%ПАРОЛЬ АДМИНИСТРАТОРА: 233444%%%%%%%" +
                "\t%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%\n" +
                "\n" +
                "\t\t\t\t  ©Юськович Влад";
            MessageBox.Show(text, "Справка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportDataOfOrders(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult res = MessageBox.Show($"Произвести экспорт данных\"?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (res == MessageBoxResult.Yes)
                {
                    OrdersData od = new OrdersData(UserID);
                    od.ExportOrders();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка во время экспорта данных.\nОшибка: {ex.Data}\n{ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            // объявление регулярных выражений
            Regex titleReg = new Regex(@"[а-я]+");
            Regex quantityReg = new Regex(@"[\d]+\-[\d]+");
            Regex priceReg = new Regex(@"[\d]+byn");
            Regex carBrandReg = new Regex(@"[a-z]+");
            // объявление строковых переменных
            string productSearchingTitle = "";
            string productSearchingQuantity = "";
            string productSearchingPrice = "";
            string productSearchingCarBrand = "";
            int price = 0;
            // поиск заголовка комплекта в полученной строковой переменной 
            foreach (Match match in titleReg.Matches(text.ToLower()))
            {
                productSearchingTitle += match + " ";
            }
            // поиск количества комплектов в полученной строковой переменной 
            foreach (Match match in quantityReg.Matches(text.ToLower()))
            {
                productSearchingQuantity += match + " ";
            }
            // поиск стоимости комплектов в полученной строковой переменной 
            foreach (Match match in priceReg.Matches(text.ToLower()))
            {
                productSearchingPrice += match + " ";
            }
            // поиск модели автомобилей для комплектов в полученной строковой переменной 
            foreach (Match match in carBrandReg.Matches(text.ToLower()))
            {
                if (match.ToString().Length >= 4 && match.ToString() != "euro")
                    productSearchingCarBrand += match + " ";
            }
            // конвертация цены, если она существует
            if (productSearchingPrice.Length != 0)
            {
                price = Convert.ToInt32(productSearchingPrice.Substring(0, productSearchingPrice.Length - 4));
            }

            return (productSearchingTitle.Trim(), productSearchingQuantity.Trim(), price, productSearchingCarBrand.Trim());
        }

        private List<ProductView> CollectProductsByFilters(string title, int minQuantity, int maxQuantity, int price, string carBrand, string fullQuery)
        {
            // объявление переменных
            List<ProductView> products = new List<ProductView>(avp.GetAllProducts()); // инициализация переменной всеми товарами из базы данных
            List<ProductView> newProducts = new List<ProductView>();
            List<ProductView> templateList = new List<ProductView>();
            if (title.Length > 0)
            {
                // поиск на совпадение заголовка у товара и фильтра
                foreach (ProductView p in products.Where(p => p.Title.ToLower().IndexOf(title.ToLower()) != -1))
                {
                    newProducts.Add(p);   
                }
            }
            if (minQuantity > 0 && maxQuantity > 0)
            {
                
                if (newProducts.Count > 0)
                {
                    // выполняем поиск определенного количества комплектов из тех комплектов, которые совпали заголовком с фитром покупателя
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
                    // выполняем поиск определенного количества комплектов из всех продуктов
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
                    // выполняем поиск определенной цены комплекта из тех комплектов, которые совпали по предыдущим фильтрам
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
                    // выполняем поиск определенной цены комплекта из всех продуктов
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
                    // выполняем поиск определенной модели автомобиля комплекта из тех комплектов, которые совпали по предыдущим фильтрам
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
                    // выполняем поиск определенной модели автомобиля комплекта из всех продуктов
                    foreach (ProductView p in products.Where(p => p.CarBrand.ToLower().IndexOf(carBrand.ToLower()) != -1))
                    {
                        newProducts.Add(p);
                    }
                }
            }
            return newProducts;
        }

        private void searchProductButton_Click(object sender, RoutedEventArgs e)
        {
            addToCartGrid.Visibility = Visibility.Hidden;
            if (!IsEditMode)
            {
                // если текст ялвяется "стандартым" по значению, то будет отображен весь список комплектов
                if (searchTextBox.Text == "Поиск")
                {
                    // очистка текущего списка для отображения
                    avp.ProductsList.Clear();
                    foreach (ProductView p in avp.GetAllProducts())
                    {
                        avp.ProductsList.Add(p);
                    }
                    sortComboBox.SelectedIndex = 0;
                }
                else
                {
                    // получаем кортеж значений из функции ParsingText, эта функция возвращает заголовок, количество, цену комплекта и модель автомобиля
                    (string, string, int, string) summaryQuery = ParcingText(searchTextBox.Text);
                    // объявление строковых переменных
                    string title = summaryQuery.Item1;
                    string quantityInterval = summaryQuery.Item2;
                    int minQuantity = quantityInterval.Length > 0 ? Convert.ToInt32(quantityInterval.Split('-')[0]) : 0;
                    int maxQuantity = quantityInterval.Length > 0 ? Convert.ToInt32(quantityInterval.Split('-')[0]) : 0;
                    int price = summaryQuery.Item3;
                    string carBrand = summaryQuery.Item4;
                    // очистка списка товаров из UI
                    avp.ProductsList.Clear();
                    // получение товаров по заданным ранее фильтрам и поплнение списка товаров в UI
                    foreach (ProductView pr in CollectProductsByFilters(title, minQuantity, maxQuantity, price, carBrand, searchTextBox.Text))
                    {
                        avp.ProductsList.Add(pr);
                    }

                }
                // отображение элемента UI об отсутствии комплектов по заданным параметрам
                if (avp.ProductsList.Count <= 0)
                {
                    captionProductItems.Visibility = Visibility.Visible;
                }
                else
                {
                    captionProductItems.Visibility = Visibility.Hidden;
                }
                // обновления листа отображения товаров
                productsListBox.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Для начала вам нужно сохранить изменения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void addProductToCart_Click(object sender, RoutedEventArgs e)
        {
            if (avp.SelectedProduct != null)
            {
                int quantity;
                try
                {
                    quantity = Convert.ToInt32(addToCartQuantity.Text);
                }
                catch
                {
                    quantity = 1;
                }
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

