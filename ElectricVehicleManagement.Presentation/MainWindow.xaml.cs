using System.Collections;
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
using ElectricVehicleManagement.Data.Models;
using ElectricVehicleManagement.Service.Cloudinary;
using ElectricVehicleManagement.Service.Listing;
using ElectricVehicleManagement.Service.User;
using Microsoft.Identity.Client;
using Microsoft.Win32;

namespace ElectricVehicleManagement.Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Auth0Client _client;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUserService _userService;
        private readonly IListingService _listingService;

        readonly string[] _connectionNames = new string[]
        {
            "Username-Password-Authentication",
            "google-oauth2"
        };

        public MainWindow(IUserService userService, ICloudinaryService cloudinaryService, IListingService listingService)
        {
            _userService = userService;
            _cloudinaryService = cloudinaryService;
            _listingService = listingService;
            InitializeComponent();
        }
        
        private void manageListingButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Please login before managing listings.",
                    "Not authenticated",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var window = new ListingManagementWindow(
                _listingService, 
                _cloudinaryService,
                App.CurrentUser!.Id            
            );

            window.Owner = this;
            window.ShowDialog();

            LoadListings();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            connectionNameComboBox.ItemsSource = _connectionNames;
            connectionNameComboBox.SelectedIndex = 0;
            LoadListings();
        }


        private async void LoadListings()
        {
            var listings = await _listingService.GetListings();
            ListingsItemsControl.ItemsSource = listings;
        }
        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            string domain = ConfigurationManager.AppSettings["Auth0:Domain"];
            string clientId = ConfigurationManager.AppSettings["Auth0:ClientId"];

            _client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = domain,
                ClientId = clientId
            });

            var extraParameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(connectionNameComboBox.Text))
                extraParameters.Add("connection", connectionNameComboBox.Text);

            var loginResult = await _client.LoginAsync(extraParameters: extraParameters);

            if (loginResult.IsError)
            {
                MessageBox.Show($"Login failed: {loginResult.Error}", "Auth0 Login", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Lấy email và avatar từ claims
            string email = loginResult.User.FindFirst(c => c.Type == "email")?.Value ?? "No email";
            string avatar = loginResult.User.FindFirst(c => c.Type == "picture")?.Value;
            string fullName = loginResult.User.FindFirst(c => c.Type == "name")?.Value;
            var currentUser = await _userService.GetOrAddUser(email, null, fullName!, null);

            App.CurrentUser = currentUser!;
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
            postListingButton.Visibility = Visibility.Visible;
            manageListingButton.Visibility = Visibility.Visible;
        }

        private void ResetHeaderAfterLogout()
        {
            loginButton.Visibility = Visibility.Visible;
            connectionNameComboBox.Visibility = Visibility.Visible;
            avatarImage.Visibility = Visibility.Collapsed;
            emailTextBlock.Visibility = Visibility.Collapsed;
            logoutButton.Visibility = Visibility.Collapsed;
            postListingButton.Visibility = Visibility.Collapsed;
            manageListingButton.Visibility =  Visibility.Collapsed;
        }

        private async void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_client != null)
            {
                var result = await _client.LogoutAsync();
                if (result != BrowserResultType.Success)
                {
                    MessageBox.Show("Logout failed", "Auth0 Logout", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }
            App.CurrentUser = null;

            ResetHeaderAfterLogout();
            MessageBox.Show("Logged out successfully");
        }


        private async void postListingButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.CurrentUser == null)
            {
                MessageBox.Show("Please login before posting a listing.",
                                "Not authenticated",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            var postWindow = new PostListingWindow(_listingService, _cloudinaryService)
            {
                Owner = this
            };

            // Nếu PostListingWindow trả DialogResult = true thì reload
            if (postWindow.ShowDialog() == true)
            {
                LoadListings();
            }
        }

    }
}