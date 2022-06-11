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
using CAR_SPARE_PARTS.Classes;
using CAR_SPARE_PARTS.Models.Order;

namespace CAR_SPARE_PARTS
{
    public partial class OrderWindow : Window
    {
        int UserID { get; set; }
        AppViewProduct avp;
        OrdersData ord;
        public OrderWindow()
        {
            InitializeComponent();
        }

        public OrderWindow(int userId)
        {
            InitializeComponent();
            UserID = userId;
            avp = new AppViewProduct(userId);
            totalPriceTextBox.Text = avp.GetOrderSum().ToString();
            ord = new OrdersData(UserID);
            Order existOrder = ord.GetExistOrder();
            if (existOrder != null)
            {
                lastNameTextBox.Text = existOrder.LastName;
                nameTextBox.Text = existOrder.Name;
                middleNameTextBox.Text = existOrder.MiddleName;
                addressTextBox.Text = existOrder.Address;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if  (addressTextBox.Text.Length > 0 && lastNameTextBox.Text.Length > 0 && middleNameTextBox.Text.Length > 0 && nameTextBox.Text.Length > 0 && calendar.Text.Length > 0)
            {
                string lastName = lastNameTextBox.Text,
                    name = nameTextBox.Text,
                    middleName = middleNameTextBox.Text,
                    address = addressTextBox.Text;
                string date = calendar.Text;
                double price = Convert.ToDouble(totalPriceTextBox.Text);
                MessageBoxResult result = MessageBox.Show($"Конечная стоимость заказа: {price}BYN на {date}.\nСовершить заказ?", "Информация",MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    ord.AddOrder(new Order
                    {
                        LastName = lastName,
                        Name = name,
                        MiddleName = middleName,
                        Address = address,
                        PriceOfOrder = price,
                        Date = date,
                        UserID = UserID
                    });
                    MessageBox.Show($"Заказ успешно выполнен, ожидайте доставку на {date} с 12:00 - 22:00", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }   
            }
            else
            {
                MessageBox.Show("Не все поля заполены верно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
