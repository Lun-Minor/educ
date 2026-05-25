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
    public partial class AddEditBookPage : Page
    {
        private BookService _bookService = new BookService();
        private GenreService _genreService = new GenreService();

        private Books _currentBook;
        private bool _isEditMode = false;

        public AddEditBookPage()
        {
            InitializeComponent();
            Loaded += AddEditBookPage_Loaded;
        }

        public AddEditBookPage(Books bookToEdit) : this()
        {
            _currentBook = bookToEdit;
            _isEditMode = true;
        }

        private void AddEditBookPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllGenres();

            if (_isEditMode && _currentBook != null) LoadExistingBookData();
        }

        private void LoadAllGenres()
        {
            GenresPanel.Children.Clear();
            var genres = _genreService.GetAllGenres();

            foreach (var g in genres)
            {
                var cb = new CheckBox
                {
                    Content = g.Name,
                    Tag = g.Id,
                    Margin = new Thickness(5),
                    FontSize = 14
                };
                GenresPanel.Children.Add(cb);
            }
        }

        private void LoadExistingBookData()
        {
            tbTitle.Text = _currentBook.Title;
            tbDescription.Text = _currentBook.Description;
            tbCoverImage.Text = _currentBook.CoverImage;
            tbTextContent.Text = _currentBook.TextContent;

            var selectedIds = _currentBook.Genres?.Select(g => g.Id).ToList() ?? new List<int>();

            foreach (var cb in GenresPanel.Children.OfType<CheckBox>())
            {
                if (cb.Tag is int id && selectedIds.Contains(id)) cb.IsChecked = true;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbTitle.Text))
            {
                MessageBox.Show("Название книги обязательно", "Ошибка");
                return;
            }

            if (_isEditMode && _currentBook != null)
            {
                _currentBook.Title = tbTitle.Text.Trim();
                _currentBook.Description = tbDescription.Text.Trim();
                _currentBook.CoverImage = tbCoverImage.Text.Trim();
                _currentBook.TextContent = tbTextContent.Text.Trim();

                Core.context.SaveChanges();

                
                var selectedIds = GetSelectedGenreIds();
                _genreService.UpdateBookGenres(_currentBook, selectedIds);

                MessageBox.Show("Книга обновлена", "Успешно");
            }
            else
            {
                
                var newBook = new Books
                {
                    Title = tbTitle.Text.Trim(),
                    Description = tbDescription.Text.Trim(),
                    CoverImage = tbCoverImage.Text.Trim(),
                    TextContent = tbTextContent.Text.Trim(),
                    AuthorId = App.CurrentUser.Id,
                    AverageRating = 0,
                    RatingCount = 0,
                    IsFrozen = false,
                    CreatedDate = DateTime.UtcNow
                };

                _bookService.AddBook(newBook);

                var selectedIds = GetSelectedGenreIds();
                _genreService.UpdateBookGenres(newBook, selectedIds);

                MessageBox.Show("Книга успешно добавлена", "Успешно");
            }

            NavigationService.GoBack();
        }

        private List<int> GetSelectedGenreIds()
        {
            return GenresPanel.Children
                .OfType<CheckBox>()
                .Where(cb => cb.IsChecked == true && cb.Tag is int)
                .Select(cb => (int)cb.Tag)
                .ToList();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}