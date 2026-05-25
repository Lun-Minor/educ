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
    public partial class MainContentPage : Page
    {
        private readonly BookService _bookService = new BookService();

        public MainContentPage()
        {
            InitializeComponent();
            DataContext = this;         
                LoadGenres();
                LoadBooks();
        }

        private void LoadBooks()
        {
            if (lbBooks != null)
            {
                lbBooks.ItemsSource = _bookService.GetAllBooksForCurrentUser();
            }
        }

        private void LoadGenres()
        {
            if (GenresPanel == null) return;

            GenresPanel.Children.Clear();

            var genres = _bookService.GetAllGenres();

            foreach (var genre in genres)
            {
                var cb = new CheckBox
                {
                    Content = genre.Name,
                    Tag = genre.Id,
                    Margin = new Thickness(5),
                    FontSize = 14
                };
                cb.Checked += (s, e) => ApplyFilters();
                cb.Unchecked += (s, e) => ApplyFilters();

                GenresPanel.Children.Add(cb);
            }
        }

        private void ApplyFilters()
{
    if (lbBooks == null) return;

    string search = tbSearch?.Text?.Trim() ?? "";

    var selectedGenres = new List<int>();
    if (GenresPanel != null)
    {
        foreach (var child in GenresPanel.Children.OfType<CheckBox>())
        {
            if (child.IsChecked == true && child.Tag is int genreId)
            {
                selectedGenres.Add(genreId);
            }
        }
    }

    List<Books> books;

   
    if (!string.IsNullOrWhiteSpace(search))
    {
        books = _bookService.SearchBooks(search);
    }
    else
    {
         books = _bookService.GetAllBooksForCurrentUser();
    }

    // Фильтрация по жанрам
    if (selectedGenres.Any())
    {
        books = books.Where(b => b.Genres.Any(g => selectedGenres.Contains(g.Id))).ToList();
    }

    string sortMode = "RatingDesc";
    if (cmbSort?.SelectedItem is ComboBoxItem sortItem && sortItem.Tag != null)
    {
        sortMode = sortItem.Tag.ToString();
    }

    switch (sortMode)
    {
        case "NameAsc": 
            books = books.OrderBy(b => b.Title).ToList(); 
            break;
        case "NameDesc": 
            books = books.OrderByDescending(b => b.Title).ToList(); 
            break;
        case "RatingAsc": 
            books = books.OrderBy(b => b.AverageRating).ToList(); 
            break;
        default: 
            books = books.OrderByDescending(b => b.AverageRating).ToList(); 
            break;
    }

    lbBooks.ItemsSource = books;
}

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void BookCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Books book)
            {
                NavigationService.Navigate(new BookPage(book));
            }
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e) { }
        private void BtnReadingLists_Click(object sender, RoutedEventArgs e)
        {
            
                NavigationService?.Navigate(new ReadingListPage());
            
        }
        private void BtnMyBooks_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AuthorPage());
        private void BtnAdmin_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AdminPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new ProfilePage());

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.GoBackToStart();
        }

        public Visibility IsAdminVisible => App.CurrentUser?.Role == 2 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsAuthorVisible => App.CurrentUser?.Role == 1 ? Visibility.Visible : Visibility.Collapsed;
    }
}