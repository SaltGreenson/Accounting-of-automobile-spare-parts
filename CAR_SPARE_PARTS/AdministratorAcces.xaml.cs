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

namespace CAR_SPARE_PARTS
{
    public partial class AdministratorAcces : Window
    {
        bool IsAdmin { get; set; }
        public AdministratorAcces()
        {
            InitializeComponent();
        }

        public bool OpenWindow() {
            this.ShowDialog();
            return IsAdmin;
        }

        private void CheckAccess_click(object sender, RoutedEventArgs e)
        {
            if (passwordTextBox.Password == "233444")
            {
                IsAdmin = true;
                MessageBox.Show("Вы стали администратором ", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            } else
            {
                MessageBox.Show("Неверный пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                passwordTextBox.Password = "";
            }
        }
    }
}
