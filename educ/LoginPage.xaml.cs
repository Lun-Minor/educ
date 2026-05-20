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
    public partial class LoginPage : Page
    {
        private readonly UserService _userService = new UserService();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text.Trim();
            string password = pbPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = _userService.Authenticate(login, password);

            if (user != null)
            {
                App.CurrentUser = user;
                MessageBox.Show($"Добро пожаловать, {user.Name}!",
                    "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.Navigate(new MainContentPage());
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.",
                    "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.GoBackToStart();
        }
    }
}