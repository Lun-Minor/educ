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
    public partial class AuthorPage : Page
    {
        private readonly BookService _bookService = new BookService();

        public AuthorPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += AuthorPage_Loaded;
        }

        private void AuthorPage_Loaded(object sender, RoutedEventArgs e)
        {           
            LoadMyBooks();
        }

        private void LoadMyBooks()
        {
            var books = _bookService.GetBooksByAuthorId(App.CurrentUser.Id);
            lbMyBooks.ItemsSource = books;
        }

        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {

            NavigationService?.Navigate(new AddEditBookPage());
        }

        private void BtnEditBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int bookId)
            {
                var book = _bookService.GetBookById(bookId);
              if (book != null) NavigationService?.Navigate(new AddEditBookPage(book));
            }
        }

        private void BtnUnfreezeBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int bookId)
            {
                string reason = Microsoft.VisualBasic.Interaction.InputBox(
                    "Напишите причину для разморозки книги:", "Оспорить заморозку книги");

                if (string.IsNullOrWhiteSpace(reason)) return;

                var request = new UnfreezeRequests
                {
                    UnfreezeBookId = bookId,
                    UserId = App.CurrentUser.Id,
                    Reason = reason,
                    Status = 0,
                    CreatedAt = DateTime.UtcNow
                };

                new UnfreezeRequestService().AddUnfreezeRequest(request);
                MessageBox.Show("Заявка на разморозку книги отправлена.", "Успешно");
            }
        }
        private void BookCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
                
            if (sender is Border border && border.DataContext is Books book)
            {
                NavigationService?.Navigate(new BookPage(book));
            }
        }
        private void BtnDeleteBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int bookId)
            {
                var book = _bookService.GetBookById(bookId);
                if (book == null)
                {
                    MessageBox.Show("Книга не найдена.", "Ошибка");
                    return;
                }

                var result = MessageBox.Show($"Вы действительно хотите удалить книгу \"{book.Title}\"?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;

                    using (var transaction = Core.context.Database.BeginTransaction())
                    {
                        // 1. Жалобы на отзывы этой книги 
                        Core.context.Complaints.RemoveRange( Core.context.Complaints.Where(c => c.ComplaintsReviewId.HasValue &&
                                Core.context.Reviews.Any(r => r.Id == c.ComplaintsReviewId.Value && r.BookId == bookId)));

                        // 2. Отзывы книги
                        Core.context.Reviews.RemoveRange(Core.context.Reviews.Where(r => r.BookId == bookId));

                        // 3. Жалобы напрямую на книгу
                        Core.context.Complaints.RemoveRange(Core.context.Complaints.Where(c => c.ComplaintsBookId == bookId));

                        // 4. Записи в списках чтения
                        Core.context.ReadingList.RemoveRange(Core.context.ReadingList.Where(r => r.BookId == bookId));

                        // 5. Заявки на разморозку книги
                        Core.context.UnfreezeRequests.RemoveRange(Core.context.UnfreezeRequests.Where(u => u.UnfreezeBookId == bookId));

                        // 6. Книга
                        var bookToDelete = Core.context.Books.FirstOrDefault(b => b.Id == bookId);
                        if (bookToDelete != null)
                        {
                            Core.context.Books.Remove(bookToDelete);
                        }

                        Core.context.SaveChanges();
                        transaction.Commit();

                        MessageBox.Show($"Книга \"{book.Title}\" успешно удалена.", "Успешно");
                        LoadMyBooks();
                    }
                
              
            }
        }
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new MainContentPage());
        private void BtnReadingLists_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new ReadingListPage());
        private void BtnMyBooks_Click(object sender, RoutedEventArgs e) { }
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