using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace educ
{

    public class UserService
    {
        public List<Users> GetAllUsers()
        {
            return Core.context.Users.OrderBy(u => u.Name).ToList();
        }

        public Users GetUserById(int id)
        {
            return Core.context.Users.FirstOrDefault(u => u.Id == id);
        }
        public Users Authenticate(string login, string password)
        {
            return Core.context.Users.FirstOrDefault(u =>u.Login == login && u.Password == password);
        }

        public bool RegisterUser(string login, string password, string name, string email)
        {
                if (Core.context.Users.Any(u => u.Login == login || u.Email == email))
                {
                    MessageBox.Show("Пользователь с таким логином или email уже существует","Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                var newUser = new Users
                {
                    Login = login,
                    Password = password,
                    Name = name,
                    Email = email,
                    Role = 0,
                    IsFrozen = false,
                    RegistrationDate = DateTime.UtcNow
                };

                Core.context.Users.Add(newUser);
                int changes = Core.context.SaveChanges();   

                if (changes > 0)
                {
                    MessageBox.Show($"Пользователь {name} добавлен!","Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                    MessageBox.Show("SaveChanges вернул 0 изменений.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            
            
        }

        public bool IsAdmin(int userId) => GetUserById(userId)?.Role == 2;
        public bool IsAuthor(int userId) => GetUserById(userId)?.Role == 1;
        public bool IsReader(int userId) => GetUserById(userId)?.Role == 0;

        public string GetUserRoleName(int role)
        {
            switch (role)
            {
                case 2:
                    return "Администратор";
                case 1:
                    return "Автор";
                case 0:
                    return "Читатель";
                default:
                    return null;
            }
        }
    }
}
    

