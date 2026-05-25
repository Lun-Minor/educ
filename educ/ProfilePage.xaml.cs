using Microsoft.VisualBasic;
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
    public partial class ProfilePage : Page
    {
        public Users CurrentUser => App.CurrentUser;
        private readonly ReviewService _reviewService = new ReviewService();
        private readonly AuthorRequestService _authorRequestService = new AuthorRequestService();
        private readonly UnfreezeRequestService _unfreezeService = new UnfreezeRequestService();

        public ProfilePage()
        {
            InitializeComponent();
            DataContext = this;        
            Loaded += ProfilePage_Loaded;
        }

        private void ProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null) return;

            
            if (App.CurrentUser.IsFrozen)
            {
                FrozenBorder.Visibility = Visibility.Visible;
                txtFreezeReason.Text = $"Причина: {App.CurrentUser.FreezeReazon ?? "Не указана"}";
            }

            btnBecomeAuthor.Visibility = (App.CurrentUser.Role == 0) ? Visibility.Visible: Visibility.Collapsed;
            LoadMyReviews();
        }

        private void LoadMyReviews()
        {
            if (App.CurrentUser == null) return;
            var reviews = _reviewService.GetReviewsByUserId(App.CurrentUser.Id);
            lbMyReviews.ItemsSource = reviews;
        }

        private void BtnBecomeAuthor_Click(object sender, RoutedEventArgs e)
        {
            string reason = Interaction.InputBox(
                "Напишите, почему вы хотите стать автором:", "Заявка на роль Автора");

            if (string.IsNullOrWhiteSpace(reason)) return;

            var request = new AuthorRequests
            {
                UserId = App.CurrentUser.Id,
                Reason = reason,
                Status = 0,
                CreatedAt = DateTime.UtcNow
            };

            _authorRequestService.AddAuthorRequest(request);
            MessageBox.Show("Заявка успешно отправлена!", "Успешно");
            btnBecomeAuthor.IsEnabled = false;
        }

        private void BtnUnfreezeRequest_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null) return;

            string reason = Interaction.InputBox("Напишите причину для разморозки аккаунта:", "Оспорить заморозку");

            if (string.IsNullOrWhiteSpace(reason)) return;

            var request = new UnfreezeRequests
            {
                UnfreezeUserId = App.CurrentUser.Id,  
                UnfreezeBookId = null,
                UserId = App.CurrentUser.Id,
                Reason = reason,
                Status = 0,
                CreatedAt = DateTime.UtcNow
            };

            _unfreezeService.AddUnfreezeRequest(request);
            MessageBox.Show("Заявка на разморозку отправлена администратору.", "Успешно");
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new MainContentPage());
        private void BtnReadingLists_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new ReadingListPage());
        private void BtnMyBooks_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AuthorPage());
        private void BtnAdmin_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AdminPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) { }
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            ((MainWindow)Application.Current.MainWindow).GoBackToStart();
        }

      
      

        public Visibility IsAdminVisible => App.CurrentUser?.Role == 2 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility IsAuthorVisible => App.CurrentUser?.Role == 1 ? Visibility.Visible : Visibility.Collapsed;

        public string RoleName
        {
            get
            {
                switch (App.CurrentUser.Role)
                {
                    case 2: return "Администратор";
                    case 1: return "Автор";
                    case 0: return "Читатель";
                    default: return "Читатель";
                }
            }
        }
    }
}