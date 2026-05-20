using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            StartScreen.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new LoginPage());
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            StartScreen.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new RegisterPage());
        }

        public void GoBackToStart()
        {
            MainFrame.Visibility = Visibility.Collapsed;
            StartScreen.Visibility = Visibility.Visible;
        }
    }
}