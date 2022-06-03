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

namespace CAR_SPARE_PARTS
{
    /// <summary>
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        int UserID { get; set; }
        AppViewProduct avp;

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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
