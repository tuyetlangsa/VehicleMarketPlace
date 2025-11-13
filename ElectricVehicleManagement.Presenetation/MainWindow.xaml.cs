using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Auth0.OidcClient;
using Duende.IdentityModel.OidcClient.Browser;
using Duende.IdentityModel.OidcClient;
using System.Configuration;
using ElectricVehicleManagement.Service.User;
using Microsoft.Identity.Client;

namespace ElectricVehicleManagement.Presenetation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Auth0Client client;
        readonly string[] _connectionNames = new string[]
        {
            "Username-Password-Authentication",
            "google-oauth2"        
        };

        private readonly IUserService _userService;
        public MainWindow(IUserService userService)
        {
            _userService = userService;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            connectionNameComboBox.ItemsSource = _connectionNames;
            connectionNameComboBox.SelectedIndex = 0;
        }
        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            string domain = ConfigurationManager.AppSettings["Auth0:Domain"];
            string clientId = ConfigurationManager.AppSettings["Auth0:ClientId"];

            client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = domain,
                ClientId = clientId
            });

            var extraParameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(connectionNameComboBox.Text))
                extraParameters.Add("connection", connectionNameComboBox.Text);

            var loginResult = await client.LoginAsync(extraParameters: extraParameters);

            if (loginResult.IsError)
            {
                MessageBox.Show($"Login failed: {loginResult.Error}", "Auth0 Login", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Lấy email và avatar từ claims
            string email = loginResult.User.FindFirst(c => c.Type == "email")?.Value ?? "No email";
            string avatar = loginResult.User.FindFirst(c => c.Type == "picture")?.Value;
            string fullName = loginResult.User.FindFirst(c => c.Type == "name")?.Value;
            await _userService.GetOrAddUser(email, null, fullName!, null);
            
            ShowHeaderAfterLogin(email, avatar);
        }



        private void ShowHeaderAfterLogin(string email, string avatarUrl)
        {
            loginButton.Visibility = Visibility.Collapsed;
            connectionNameComboBox.Visibility = Visibility.Collapsed;

            emailTextBlock.Text = email;
            emailTextBlock.Visibility = Visibility.Visible;

            if (!string.IsNullOrEmpty(avatarUrl))
            {
                try
                {
                    avatarImage.Source = new BitmapImage(new Uri(avatarUrl));
                    avatarImage.Visibility = Visibility.Visible;
                }
                catch
                {
                    avatarImage.Visibility = Visibility.Collapsed;
                }
            }

            logoutButton.Visibility = Visibility.Visible;
        }

        private void ResetHeaderAfterLogout()
        {
            loginButton.Visibility = Visibility.Visible;
            connectionNameComboBox.Visibility = Visibility.Visible;

            avatarImage.Visibility = Visibility.Collapsed;
            emailTextBlock.Visibility = Visibility.Collapsed;

            logoutButton.Visibility = Visibility.Collapsed;
        }

        private async void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                var result = await client.LogoutAsync();
                if (result != BrowserResultType.Success)
                {
                    MessageBox.Show("Logout failed", "Auth0 Logout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            ResetHeaderAfterLogout();
        }
    }
}