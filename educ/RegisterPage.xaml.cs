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

namespace educ
{
    public partial class RegisterPage : Page
    {
        private readonly UserService _userService = new UserService();

        public RegisterPage()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string name = tbName.Text.Trim();
            string login = tbLogin.Text.Trim();
            string email = tbEmail.Text.Trim();
            string password = pbPassword.Password;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Все поля обязательны для заполнения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success = _userService.RegisterUser(login, password, name, email);

            if (success)
            {
                NavigationService.Navigate(new LoginPage());
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
            else
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.GoBackToStart();
            }
        }
    }
}