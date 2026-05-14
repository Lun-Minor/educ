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
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void testbutton_Click(object sender, RoutedEventArgs e)
        {
            var data = new DataFromSql();
            var books = data.GetAllBooks();

            string text = $"books {books.Count}\n";

            foreach (var g in books)
            {
                text += $" {g.Id} - {g.Title}\n";
            }

            MessageBox.Show(text, "успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
