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
using CAR_SPARE_PARTS;
using System.Threading;
namespace CAR_SPARE_PARTS.Pages
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class SignIn : Page
    {
        public SignIn()
        {
            InitializeComponent();
        }

        private void Login_click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = loginTextBox.Text;
                string password = passwordTextBox.Password;
                if (login.Length > 0 && password.Length > 0)
                {
                    Autorize autorize = new Autorize(login, password);
                    (string, bool, bool) signIn = autorize.SignIn();
                    if (signIn.Item2)
                    {
                        
                        StoreWindow storeWindow = new StoreWindow(signIn.Item3);
                        storeWindow.Show();
                        MessageBox.Show(signIn.Item3? $"Вы вошли в приложение под логином администратора \"{login}\"" : $"Вы вошли в приложение под логином пользователя \"{login}\".", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        Application.Current.MainWindow.Close();
                    }
                    else
                    {
                        MessageBox.Show(signIn.Item1, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        passwordTextBox.Password = "";
                    }
                    autorize = null;
                }
                else
                {
                    MessageBox.Show("Вам необходимо заполнить поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.Data, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
