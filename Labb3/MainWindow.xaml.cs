using System.Windows;
using Labb3ProgTemplate.Managers;

namespace Labb3ProgTemplate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UserManager.CurrentUserChanged += UserManager_CurrentUserChanged;

            AdminTab.Visibility = Visibility.Collapsed;
            ShopTab.Visibility = Visibility.Collapsed;
            LoginTab.Visibility = Visibility.Visible;

            Tabs.SelectedItem = LoginTab;
        }

        private void UserManager_CurrentUserChanged()
        {
            if (UserManager.CurrentUser.LoggedIn == false)
            {
                AdminTab.Visibility = Visibility.Collapsed;
                ShopTab.Visibility = Visibility.Collapsed;
                LoginTab.Visibility = Visibility.Visible;

                Tabs.SelectedItem = LoginTab;
            }
            else if (UserManager.IsAdminLoggedIn)
            {
                AdminTab.Visibility = Visibility.Visible;
                ShopTab.Visibility = Visibility.Visible;
                LoginTab.Visibility = Visibility.Collapsed;

                Tabs.SelectedItem = AdminTab;
            }
            else
            {
                ShopTab.Visibility = Visibility.Visible;
                AdminTab.Visibility = Visibility.Collapsed;
                LoginTab.Visibility = Visibility.Collapsed;

                Tabs.SelectedItem = ShopTab;
            }
        }
    }
}
