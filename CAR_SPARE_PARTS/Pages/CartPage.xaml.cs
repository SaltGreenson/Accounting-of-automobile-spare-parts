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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CAR_SPARE_PARTS.Classes;

namespace CAR_SPARE_PARTS.Pages
{
    /// <summary>
    /// Interaction logic for CartPage.xaml
    /// </summary>
    public partial class CartPage : Page
    {
        AppViewProduct avp;
        private int UserID { get; set; }
        public CartPage()
        {
            InitializeComponent();
        }

        public CartPage(int userId)
        {
            InitializeComponent();
            try
            {
                UserID = userId;
                avp = new AppViewProduct(userId, true, false);
                DataContext = avp;
            } catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void removeFromCartButton_Click(object sender, RoutedEventArgs e)
        {
            int quantity = -1;
            if (removeFromCartQuantityTextBox.Text.Length == 0)
            {
                quantity = avp.SelectedProduct.Quantity;
            }
            try
            {
                if (quantity == -1)
                    quantity = Convert.ToInt32(removeFromCartQuantityTextBox.Text) <= avp.SelectedProduct.Quantity ? Convert.ToInt32(removeFromCartQuantityTextBox.Text) : avp.SelectedProduct.Quantity;
            }
            catch
            {
                quantity = 1;
                removeFromCartQuantityTextBox.Text = quantity.ToString();
            }

            
                string title = avp.SelectedProduct.Title;
                MessageBoxResult res = MessageBox.Show($"Вы действительно хотите удалить \"{title}\" {quantity}шт.?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (res == MessageBoxResult.Yes)
                {
                    avp.RemoveFromCart(quantity);
                    cartListBox.Items.Refresh();
                }
            

        }
    }
}
