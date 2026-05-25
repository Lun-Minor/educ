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
            DataContext = _currentBook;
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

            txtGenres.Text = _currentBook.Genres != null && _currentBook.Genres.Any()? string.Join
                (", ", _currentBook.Genres.Select(g => g.Name)): "Жанры не указаны";

            
            
            bool isAdmin = App.CurrentUser?.Role == 2;
            bool isAuthor = App.CurrentUser?.Id == _currentBook.AuthorId;
            bool isFrozen = _currentBook.IsFrozen;

            if (isFrozen && (isAdmin || isAuthor))
            {
                txtBookFrozen.Visibility = Visibility.Visible;

                if (!string.IsNullOrEmpty(_currentBook.FreezeReazon))
                {
                    txtFreezeReason.Text = $"Причина заморозки: {_currentBook.FreezeReazon}";
                    txtFreezeReason.Visibility = Visibility.Visible;
                }
            }
            else
            {
                txtBookFrozen.Visibility = Visibility.Collapsed;
                txtFreezeReason.Visibility = Visibility.Collapsed;
            }
        }
    

        private void LoadReviews()
        {
            var reviews = _reviewService.GetReviewsByBookId(_currentBook.Id);
            lbReviews.ItemsSource = reviews;
        }



        private void BtnComplainBook_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser?.IsFrozen == true)
            {
                MessageBox.Show("Ваш аккаунт заморожен. Вы не можете отправлять жалобы.", "Аккаунт заморожен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string reason = Microsoft.VisualBasic.Interaction.InputBox("Введите причину жалобы на книгу:", "Жалоба на книгу");
            if (string.IsNullOrWhiteSpace(reason)) return;

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
            if (App.CurrentUser?.IsFrozen == true)
            {
                MessageBox.Show("Ваш аккаунт заморожен. Вы не можете отправлять жалобы.", "Аккаунт заморожен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string reason = Microsoft.VisualBasic.Interaction.InputBox("Введите причину жалобы на автора:", "Жалоба на автора");
            if (string.IsNullOrWhiteSpace(reason)) return;

          
            var complaint = new Complaints
            {
                UserId = App.CurrentUser.Id,
                ComplaintsUserId = _currentBook.AuthorId,   
                Reason = reason,
                Status = 0,
                CreatedAt = DateTime.UtcNow
            };

            _complaintService.AddComplaint(complaint);
            MessageBox.Show("Жалоба на автора успешно отправлена", "Успешно");
        }


        private void BtnBack_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();

        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBook != null)
            {
                NavigationService?.Navigate(new ReadBookPage(_currentBook));
            }
        }

        private void BtnAddReview_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser?.IsFrozen == true)
            {
                MessageBox.Show("Ваш аккаунт заморожен. Вы не можете оставлять отзывы.\n" + $"Причина: {App.CurrentUser.FreezeReazon}",
                               "Аккаунт заморожен", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(tbReviewText.Text))
            {
                MessageBox.Show("Напишите текст отзыва.", "Ошибка");
                return;
            }
            bool alreadyReviewed = Core.context.Reviews.Any(r =>r.UserId == App.CurrentUser.Id &&r.BookId == _currentBook.Id);

            if (alreadyReviewed)
            {
                MessageBox.Show("Вы уже оставляли отзыв на эту книгу.Повторная публикация отзыва запрещена.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
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

            MessageBox.Show("Отзыв успешно опубликован", "Успех");
            tbReviewText.Clear();
            LoadReviews();
        }


        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new MainContentPage());
        private void BtnReadingLists_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ReadingListPage());
        private void BtnMyBooks_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new AuthorPage());
        private void BtnAdmin_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AdminPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new ProfilePage());

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
                Width = 340,
                Height = 240
            };

            var stack = new StackPanel { Margin = new Thickness(15) };

            stack.Children.Add(new TextBlock
            {
                Text = $"Книга: {_currentBook.Title}",
                FontWeight = FontWeights.SemiBold,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10)
            });

            _tempStatusComboBox = new ComboBox { Height = 40, Margin = new Thickness(10) };
            _tempStatusComboBox.Items.Add(new ComboBoxItem { Content = "Заброшено", Tag = 0 });
            _tempStatusComboBox.Items.Add(new ComboBoxItem { Content = "В планах", Tag = 1 });
            _tempStatusComboBox.Items.Add(new ComboBoxItem { Content = "Читаю", Tag = 2 });
            _tempStatusComboBox.Items.Add(new ComboBoxItem { Content = "Прочитано", Tag = 3 });

            _tempStatusComboBox.SelectedIndex = 2; 

            stack.Children.Add(_tempStatusComboBox);

            var btnAdd = new Button
            {
                Content = "Добавить в список",
                Height = 45,
                FontSize = 14
            };
            btnAdd.Click += BtnAddToReadingList_Click;

            stack.Children.Add(btnAdd);
            _tempAdd.Content = stack;
            _tempAdd.ShowDialog();
        }

        private void BtnAddToReadingList_Click(object sender, RoutedEventArgs e)
        {

            if (_tempStatusComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is int status)
            {
                var service = new ReadingListService();

                var item = new ReadingList
                {
                    UserId = App.CurrentUser.Id,
                    BookId = _currentBook.Id,
                    Status = status,
                    AddedDate = DateTime.UtcNow
                };

                service.AddToReadingList(item);

                string statusName;
                switch (status)
                {
                    case 0: statusName = "Заброшено"; break;
                    case 1: statusName = "В планах"; break;
                    case 2: statusName = "Читаю"; break;
                    case 3: statusName = "Прочитано"; break;
                    default: statusName = "Читаю"; break;
                }

                MessageBox.Show($"Книга: {_currentBook.Title} добавлена в список: {statusName}", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);

                _tempAdd.Close();
                _tempAdd = null;
                _tempStatusComboBox = null;
            }
        }


        private void BtnFreezeBook_Click(object sender, RoutedEventArgs e)
        {
            string reason = Microsoft.VisualBasic.Interaction.InputBox("Введите причину заморозки книги:", "Заморозка книги");

            if (string.IsNullOrWhiteSpace(reason)) return;

            _currentBook.IsFrozen = true;
            _currentBook.FreezeReazon = reason;

            Core.context.SaveChanges();

            MessageBox.Show("Книга успешно заморожена.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
 
        private void BtnFreezeReview_Click(object sender, RoutedEventArgs e)
        {
            
            if (sender is Button btn && btn.Tag is int reviewId)
            {
                string reason = Microsoft.VisualBasic.Interaction.InputBox("Введите причину заморозки отзыва:", "Заморозка отзыва");

                if (string.IsNullOrWhiteSpace(reason)) { return; } _reviewService.FreezeReview(reviewId, reason);
                MessageBox.Show("Отзыв успешно заморожен.", "Успешно");
                LoadReviews(); 
            }
        }
       
        private void BtnComplainReview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int reviewId)
            {
                string reason = Microsoft.VisualBasic.Interaction.InputBox("Введите причину жалобы на отзыв:", "Жалоба на отзыв");

                if (string.IsNullOrWhiteSpace(reason)) return;

                var complaint = new Complaints
                {
                    UserId = App.CurrentUser.Id,
                    ComplaintsReviewId = reviewId,
                    Reason = reason,
                    Status = 0,
                    CreatedAt = DateTime.UtcNow
                };

                _complaintService.AddComplaint(complaint);
                MessageBox.Show("Жалоба на отзыв успешно отправлена", "Успешно");
            }
        }
        public Visibility IsAdminVisible => App.CurrentUser?.Role == 2 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsAuthorVisible => App.CurrentUser?.Role == 1 ? Visibility.Visible : Visibility.Collapsed;
    
    }
}