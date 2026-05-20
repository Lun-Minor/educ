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
    public partial class BookPage : Page
    {
        private readonly Books _currentBook;
        private readonly ReviewService _reviewService = new ReviewService();
        private readonly ComplaintService _complaintService = new ComplaintService();

        public BookPage(Books book)
        {
            InitializeComponent();
            _currentBook = book;
            DataContext = this;

            LoadBookData();
            LoadReviews();
        }

        private void LoadBookData()
        {
            if (_currentBook == null) return;

            txtTitle.Text = _currentBook.Title;
            txtDescription.Text = _currentBook.Description ?? "Описание отсутствует.";
            txtRating.Text = $"⭐ {_currentBook.AverageRating}";
            txtAuthor.Text = _currentBook.Users?.Name ?? "Автор неизвестен";

            txtGenres.Text = _currentBook.Genres != null && _currentBook.Genres.Any()
                ? string.Join(", ", _currentBook.Genres.Select(g => g.Name))
                : "Жанры не указаны";

            if (!string.IsNullOrEmpty(_currentBook.CoverImage))
            {
                imgCover.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(_currentBook.CoverImage));
            }
        }

        private void LoadReviews()
        {
            var reviews = _reviewService.GetReviewsByBookId(_currentBook.Id);
            lbReviews.ItemsSource = reviews;
        }

        

        private void BtnComplainBook_Click(object sender, RoutedEventArgs e)
        {
            string reason = Microsoft.VisualBasic.Interaction.InputBox("Введите причину жалобы на книгу:", "Жалоба на книгу");

            if (string.IsNullOrWhiteSpace(reason)) { return; }
                
            var complaint = new Complaints
            {
                UserId = App.CurrentUser.Id,
                ComplaintsBookId = _currentBook.Id,
                Reason = reason,
                Status = 0,
                CreatedAt = DateTime.UtcNow
            };

            _complaintService.AddComplaint(complaint);
            MessageBox.Show("Жалоба на книгу успешно отправлена", "Успешно");
        }

        private void BtnComplainAuthor_Click(object sender, RoutedEventArgs e)
        {
            string reason = Microsoft.VisualBasic.Interaction.InputBox("Введите причину жалобы на автора:","Жалоба на автора");

            if (string.IsNullOrWhiteSpace(reason)) { return; }
            MessageBox.Show("Жалоба на автора отправлена", "Информация");
        }


        private void BtnBack_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();

        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(_currentBook.TextContent ?? "Текст книги пока не добавлен.", $"Читаем: {_currentBook.Title}");
        }

        private void BtnAddReview_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbReviewText.Text))
            {
                MessageBox.Show("Напишите текст отзыва.", "Ошибка");
                return;
            }

            int rating = int.Parse((cmbRating.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "5");

            var review = new Reviews
            {
                BookId = _currentBook.Id,
                UserId = App.CurrentUser.Id,
                Text = tbReviewText.Text.Trim(),
                Rating = rating,
                CreatedAt = DateTime.UtcNow,
                IsFrozen = false
            };

            _reviewService.AddReview(review);
            MessageBox.Show("Отзыв опубликован", "Успех");
            tbReviewText.Clear();
            LoadReviews();
        }

    
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainContentPage());
        private void BtnReadingLists_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Мои списки — в разработке");
        private void BtnMyBooks_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Мои книги - в разработке");
        private void BtnAdmin_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Администрирование — в разработке");
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Профиль — в разработке");

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.GoBackToStart();
        }

      
        private ComboBox _tempStatusComboBox;
        private Window _tempAdd;

        private void BtnAddToList_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBook == null || App.CurrentUser == null) return;

            _tempAdd = new Window
            {
                Title = "Добавить в список",
                Width = 320,
                Height = 220
            };

            var stack = new StackPanel { Margin = new Thickness(5) };

            stack.Children.Add(new TextBlock
            {
                Text = $"Книга: {_currentBook.Title}",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(5)
            });

            _tempStatusComboBox = new ComboBox { Height = 35 };
            _tempStatusComboBox.Items.Add(new ComboBoxItem { Content = "Заброшено", Tag = 0 });
            _tempStatusComboBox.Items.Add(new ComboBoxItem { Content = "В планах", Tag = 1 });
            _tempStatusComboBox.Items.Add(new ComboBoxItem { Content = "Читаю", Tag = 2 });
            _tempStatusComboBox.Items.Add(new ComboBoxItem { Content = "Прочитано", Tag = 3 });

            _tempStatusComboBox.SelectedIndex = 1;

            stack.Children.Add(_tempStatusComboBox);

            var btnAdd = new Button
            {
                Content = "Добавить",
                Height = 40,
                Margin = new Thickness(5)
            };

            btnAdd.Click += BtnAddToReadingList_Click;  

            stack.Children.Add(btnAdd);
            _tempAdd.Content = stack;
            _tempAdd.ShowDialog();
        }

        private void BtnAddToReadingList_Click(object sender, RoutedEventArgs e)
        {
            if (_tempStatusComboBox == null || _tempAdd == null) return;

            var selectedItem = _tempStatusComboBox.SelectedItem as ComboBoxItem;
           

            int status = (int)selectedItem.Tag;

            var readingListService = new ReadingListService();

            var item = new ReadingList
            {
                UserId = App.CurrentUser.Id,
                BookId = _currentBook.Id,
                Status = status,
                AddedDate = DateTime.UtcNow
            };

            readingListService.AddToReadingList(item);

            MessageBox.Show("Книга успешно добавлена в список", "Успешно");
            _tempAdd.Close();
            _tempStatusComboBox = null;
            _tempAdd = null;
        }

        public Visibility IsAdminVisible => App.CurrentUser?.Role == 2 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsAuthorVisible => App.CurrentUser?.Role == 1 ? Visibility.Visible : Visibility.Collapsed;

    }
}