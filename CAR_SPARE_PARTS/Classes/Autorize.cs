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

        public (string,bool, bool) SignIn()
        {
            using (var dbContext = new UserContext())
            {
                IQueryable<User> currentUser = dbContext.Users.Where(user => user.Login == Login);
                if (currentUser.Count() > 0)
                {
                    User user = currentUser.First();
                    return (user.Password == Password? $"Пользователь {user.Login} успешно вошел в программу" : $"Неверный пароль", user.Password == Password, user.IsAdministrator);
                }
            }
            return ($"Неверный логин", false, false);
        }

        public (string, bool) SignUp()
        {
            using (var dbContext = new UserContext())
            {
                bool isUser = dbContext.Users.Where(user => user.Login == Login).Count() > 0;
                if (!isUser)
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
