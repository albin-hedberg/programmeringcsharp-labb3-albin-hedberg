using System.Windows;
using System.Windows.Controls;
using Labb3ProgTemplate.Enums;
using Labb3ProgTemplate.Managers;

namespace Labb3ProgTemplate.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            UserManager.CurrentUserChanged += UserManager_CurrentUserChanged;
            UserManager.UserDoesntExist += UserManager_UserDoesntExist;
            UserManager.UserAlreadyExist += UserManager_UserAlreadyExist;
            UserManager.UserAuthenticationFailed += UserManager_UserAuthenticationFailed;
        }

        private void UserManager_CurrentUserChanged()
        {
            LoginName.Text = string.Empty;
            LoginPwd.Password = string.Empty;
            RegisterName.Text = string.Empty;
            RegisterPwd.Password = string.Empty;
            ErrorMessage.Text = string.Empty;
        }

        #region ErrorCheckingFunctions

        private void UserManager_UserDoesntExist()
        {
            ErrorMessage.Text = "User doesn't exist, register a new user?";
            RegisterName.Text = LoginName.Text;
            RegisterPwd.Password = LoginPwd.Password;
        }

        private void UserManager_UserAlreadyExist()
        {
            ErrorMessage.Text = "User already exist, login instead?";
            LoginName.Text = RegisterName.Text;
            LoginPwd.Password = RegisterPwd.Password;
        }

        private void UserManager_UserAuthenticationFailed()
        {
            ErrorMessage.Text = "Password is incorrect, try again.";
        }

        private bool CheckInputLength(string name, string password)
        {
            if (name.Length < 1 || password.Length < 1)
            {
                ErrorMessage.Text = "All fields aren't filled in.";
                return false;
            }

            return true;
        }

        #endregion

        private void LoginBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (CheckInputLength(LoginName.Text, LoginPwd.Password))
            {
                UserManager.Login(LoginName.Text, LoginPwd.Password);
            }
        }

        private void RegisterAdminBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (CheckInputLength(RegisterName.Text, RegisterPwd.Password))
            {
                UserManager.RegisterNewUser(RegisterName.Text, RegisterPwd.Password, UserTypes.Admin);
            }
        }

        private void RegisterCustomerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CheckInputLength(RegisterName.Text, RegisterPwd.Password))
            {
                UserManager.RegisterNewUser(RegisterName.Text, RegisterPwd.Password, UserTypes.Customer);
            }
        }
    }
}
