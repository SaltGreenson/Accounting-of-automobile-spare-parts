using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAR_SPARE_PARTS.Models.User;


namespace CAR_SPARE_PARTS
{
    public class Autorize
    {
        string Login { get; set; }
        string Password { get; set; }
        string RepeatPassword { get; set; }

        bool IsAdmin { get; set; }

        public Autorize(string login, string password, string repeatPassword, bool isAdmin)
        {
            Login = login;
            Password = password;
            RepeatPassword = repeatPassword;
            IsAdmin = isAdmin;
        }
        public Autorize(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public (string,bool, bool, int) SignIn()
        {
            User currentUser;
            using (var dbContext = new UserContext())
            {
                currentUser = dbContext.Users.SingleOrDefault(user => user.Login == Login);
                if (currentUser != null)
                {
                    return (currentUser.Password == Password? $"Пользователь {currentUser.Login} успешно вошел в программу" : $"Неверный пароль", currentUser.Password == Password, currentUser.IsAdministrator, currentUser.ID);
                }
            }
            return ($"Неверный логин", false, false, 0);
        }

        public (string, bool) SignUp()
        {
            using (var dbContext = new UserContext())
            {
                User existUser = dbContext.Users.SingleOrDefault(user => user.Login == Login);
                if (existUser == null)
                {
                    if (Password == RepeatPassword)
                    {
                        if (Password.Length > 8)
                        {
                            dbContext.Users.Add(new User(Login, Password, IsAdmin));
                            dbContext.SaveChanges();
                            return ($"Пользователь {Login} успешно зарегистрирован", true);

                        }
                        return ("Длина пароля должна быть больше 8 символов", false);

                    }
                    return ($"Пароли не совпадают: {Password} != {RepeatPassword}", false);


                }
            }
            return ("Данный логин уже существует", false);
        }
    }
}
