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
    public class ReadingListItem
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookAuthor { get; set; }
        public decimal BookRating { get; set; }
        public string BookCover { get; set; }
    }

    public partial class ReadingListPage : Page
    {
        private readonly ReadingListService _readingListService = new ReadingListService();
        private readonly BookService _bookService = new BookService();
        private int _currentStatus = 2;

        public ReadingListPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadGenres();
            LoadReadingList();
        }



        private void LoadReadingList()
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Вы не авторизованы", "Ошибка");
                return;
            }

            var selectedGenres = GetSelectedGenreIds();
            string sortMode = GetSortMode();

            var items = _readingListService.GetFilteredReadingList(
                App.CurrentUser.Id,
                _currentStatus,
                tbSearch?.Text?.Trim() ?? "",
                selectedGenres,
                sortMode
            ) ?? new List<ReadingList>();

            var displayList = new List<ReadingListItem>();

            foreach (var r in items)
            {
                if (r?.Books == null) continue;

                var book = r.Books;

     
                if (book.Users?.IsFrozen == true)
                    continue;

            
                displayList.Add(new ReadingListItem
                {
                    BookId = r.BookId,
                    BookTitle = book.Title,
                    BookAuthor = book.Users?.Name,
                    BookRating = book.AverageRating,
                    BookCover = book.CoverImage
                });
            }

            if (lbReadingList != null)
                lbReadingList.ItemsSource = displayList;
        }

        private List<int> GetSelectedGenreIds()
        {
            if (GenresPanel == null) return new List<int>();
            return GenresPanel.Children.OfType<CheckBox>().Where(cb => cb.IsChecked == true && cb.Tag is int).Select(cb => (int)cb.Tag).ToList();
        }

        private string GetSortMode()
        {
            if (cmbSort?.SelectedItem is ComboBoxItem item && item.Tag != null) return item.Tag.ToString();
            return "RatingDesc";
        }

        private void LoadGenres()
        {
            if (GenresPanel == null) return;

            GenresPanel.Children.Clear();
            var genres = _bookService.GetAllGenres() ?? new List<Genres>();

            foreach (var genre in genres)
            {
                var cb = new CheckBox
                {
                    Content = genre.Name,
                    Tag = genre.Id,
                    Margin = new Thickness(10),
                    FontSize = 14
                };
                cb.Checked += (s, e) => LoadReadingList();
                cb.Unchecked += (s, e) => LoadReadingList();
                GenresPanel.Children.Add(cb);
            }
        }

        

        private void cmbListType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbListType.SelectedItem is ComboBoxItem item && item.Tag != null && int.TryParse(item.Tag.ToString(), out int status))
            {
                _currentStatus = status;
                LoadReadingList();
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadReadingList();
        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadReadingList();

        private void BookCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is ReadingListItem item)
            {
                var book = _bookService.GetBookById(item.BookId);
                if (book != null) NavigationService?.Navigate(new BookPage(book));
            }
        }

        private void BookCard_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is ReadingListItem item)
            {
                ShowMoveWindow(item);
            }
        }

        private Window _tempMoveWindow;
        private ComboBox _tempStatusComboBoxMove;
        private ReadingListItem _selectedItemForMove;

        private void ShowMoveWindow(ReadingListItem item)
        {
            _selectedItemForMove = item;
            _tempMoveWindow = new Window
            {
                Title = "Переместить книгу",
                Width = 340,
                Height = 240
            };

            var stack = new StackPanel { Margin = new Thickness(15) };
            stack.Children.Add(new TextBlock { Text = $"Книга: {item.BookTitle}", FontWeight = FontWeights.SemiBold, Margin = new Thickness(0,0,0,10), TextWrapping = TextWrapping.Wrap });

            _tempStatusComboBoxMove = new ComboBox { Height = 40, Margin = new Thickness(10) };
            _tempStatusComboBoxMove.Items.Add(new ComboBoxItem { Content = "Заброшено", Tag = 0 });
            _tempStatusComboBoxMove.Items.Add(new ComboBoxItem { Content = "В планах", Tag = 1 });
            _tempStatusComboBoxMove.Items.Add(new ComboBoxItem { Content = "Читаю", Tag = 2 });
            _tempStatusComboBoxMove.Items.Add(new ComboBoxItem { Content = "Прочитано", Tag = 3 });
            _tempStatusComboBoxMove.SelectedIndex = _currentStatus;

            stack.Children.Add(_tempStatusComboBoxMove);

            var btnMove = new Button
            {
                Content = "Переместить",
                Height = 45               
            };
            btnMove.Click += BtnMoveBook_Click;
            stack.Children.Add(btnMove);

            _tempMoveWindow.Content = stack;
            _tempMoveWindow.ShowDialog();
        }

        private void BtnMoveBook_Click(object sender, RoutedEventArgs e)
        {
            if (_tempStatusComboBoxMove?.SelectedItem is ComboBoxItem si && si.Tag is int newStatus && _selectedItemForMove != null)
            {
                _readingListService.ChangeBookStatus(App.CurrentUser?.Id ?? 0, _selectedItemForMove.BookId, newStatus);
                LoadReadingList();
            }

            _tempMoveWindow?.Close();
            _tempMoveWindow = null;
            _tempStatusComboBoxMove = null;
            _selectedItemForMove = null;
        }

       
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new MainContentPage());
        private void BtnReadingLists_Click(object sender, RoutedEventArgs e) { }
        private void BtnMyBooks_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AuthorPage());
        private void BtnAdmin_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AdminPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new ProfilePage());

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            ((MainWindow)Application.Current.MainWindow).GoBackToStart();
        }

        public Visibility IsAdminVisible => App.CurrentUser?.Role == 2 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsAuthorVisible => App.CurrentUser?.Role == 1 ? Visibility.Visible : Visibility.Collapsed;

       
    }
}