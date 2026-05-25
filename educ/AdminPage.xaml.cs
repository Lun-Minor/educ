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
using Microsoft.VisualBasic;

///summary
/// панель админисратора
///summary


namespace educ
{
    public partial class AdminPage : Page
    {
        private readonly AdminService _adminService = new AdminService();

        public AdminPage()
        {
            InitializeComponent();
            Loaded += AdminPage_Loaded;
        }

        private void AdminPage_Loaded(object sender, RoutedEventArgs e)
        {
            cmbFilter.SelectedIndex = 0;
            LoadData();
        }

        private void cmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            if (lbItems == null) return;

            if (cmbFilter.SelectedItem is ComboBoxItem item && item.Tag is string filter)
            {
                lbItems.ItemsSource = _adminService.GetDataForFilter(filter);
            }
        }


        private void BtnApprove_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                _adminService.ApproveItem(btn.Tag, App.CurrentUser);
                LoadData();
            }
        }

        private void BtnReject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                _adminService.RejectItem(btn.Tag, App.CurrentUser);
                LoadData();
            }
        }

        private void BtnUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                _adminService.UnfreezeItem(btn.Tag);
                LoadData();
            }
        }

        private void BtnFreezeUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                object realObj = _adminService.GetObjectFromItem(btn.Tag);
                if (realObj is Users user)
                {
                    _adminService.FreezeUser(user);
                    LoadData();
                }
            }
        }

        private void BtnChangeRole_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                object realObj = _adminService.GetObjectFromItem(btn.Tag);
                if (realObj is Users user)
                {
                    _adminService.ChangeUserRole(user);
                    LoadData();
                }
            }
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                object realObj = _adminService.GetObjectFromItem(btn.Tag);
                if (realObj is Users user)
                {
                    _adminService.ChangeUserPassword(user);
                    LoadData();
                }
            }
        }



        /// <summary>
        /// навигация
        /// </summary>
        
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new MainContentPage());
        private void BtnReadingLists_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new ReadingListPage());
        private void BtnAdmin_Click(object sender, RoutedEventArgs e) { }
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