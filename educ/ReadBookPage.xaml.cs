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
    public partial class ReadBookPage : Page
    {
        private readonly Books _currentBook;

        public ReadBookPage(Books book)
        {
            InitializeComponent();
            _currentBook = book;
            DataContext = this;    
            

            if (_currentBook != null)
            {
                txtBookTitle.Text = _currentBook.Title;
                txtBookContent.Text = _currentBook.TextContent;
            }
        }
 
        private void BtnCatalog_Click(object sender, RoutedEventArgs e)=> NavigationService?.Navigate(new MainContentPage());

        private void BtnReadingLists_Click(object sender, RoutedEventArgs e)=> NavigationService?.Navigate(new ReadingListPage());

        private void BtnMyBooks_Click(object sender, RoutedEventArgs e)=> NavigationService?.Navigate(new AuthorPage());

        private void BtnAdmin_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AdminPage());

        private void BtnProfile_Click(object sender, RoutedEventArgs e)=> NavigationService?.Navigate(new ProfilePage());

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            ((MainWindow)Application.Current.MainWindow).GoBackToStart();
        }

       
        public Visibility IsAdminVisible => App.CurrentUser?.Role == 2 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsAuthorVisible => App.CurrentUser?.Role == 1 ? Visibility.Visible : Visibility.Collapsed;
    }
}