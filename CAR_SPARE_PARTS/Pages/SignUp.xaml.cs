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
using CAR_SPARE_PARTS.Models.User;

namespace CAR_SPARE_PARTS.Pages
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Page
    {
        bool wasQuestion = false;
        
        public SignUp()
        {
            InitializeComponent();
        }

        private void Registration_click(object sender, RoutedEventArgs e)
        {
            try
            {


                string login = loginTextBox.Text;
                string password = passwordTextBox.Password;
                string repeatPassword = repeatPasswordTextBox.Password;
                bool isAdmin = false;
                if (login.Length > 0 && password.Length > 0 && repeatPassword.Length > 0)
                {
                    if (!wasQuestion)
                    {
                        MessageBoxResult result = MessageBox.Show("Желаете получить доступ администратора?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            wasQuestion = true;
                            AdministratorAcces window = new AdministratorAcces();
                            isAdmin = window.OpenWindow();
                        }
                    }
                    Autorize autorize = new Autorize(login, password, repeatPassword, isAdmin);
                    (string, bool) signUp = autorize.SignUp();
                    if (signUp.Item2)
                    {
                       NavigationService.Navigate(new SignIn());
                    }
                    else
                    {
                        MessageBox.Show(signUp.Item1, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        passwordTextBox.Password = "";
                        repeatPasswordTextBox.Password = "";

                    }
                    autorize = null;
                }
                else
                {
                    MessageBox.Show("Вам необходимо заполнить поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.Data, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void loginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            wasQuestion = false;
        }
    }
}
