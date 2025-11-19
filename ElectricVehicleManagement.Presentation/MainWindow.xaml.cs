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
using ElectricVehicleManagement.Data.Models.Enums;
using ElectricVehicleManagement.Service.Cloudinary;
using ElectricVehicleManagement.Service.Listing;
using ElectricVehicleManagement.Service.User;
using Microsoft.Extensions.DependencyInjection;
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
        private List<Listing> AllListings = new List<Listing>();
        private List<Listing> FilteredListings = new List<Listing>();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            connectionNameComboBox.ItemsSource = _connectionNames;
            connectionNameComboBox.SelectedIndex = 0;
            InitEnumCombos();
            LoadListings();
        }


        private async void LoadListings()
        {
            var listings = await _listingService.GetListings();
            AllListings = listings.ToList(); // Lưu danh sách gốc
            ListingsItemsControl.ItemsSource = AllListings;
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
        }

        private void ResetHeaderAfterLogout()
        {
            loginButton.Visibility = Visibility.Visible;
            connectionNameComboBox.Visibility = Visibility.Visible;
            avatarImage.Visibility = Visibility.Collapsed;
            emailTextBlock.Visibility = Visibility.Collapsed;
            logoutButton.Visibility = Visibility.Collapsed;
            postListingButton.Visibility = Visibility.Collapsed;
        }

        private void InitEnumCombos()
        {
            BodyTypeComboBox.ItemsSource = Enum.GetValues(typeof(BodyType));
            EnergyComboBox.ItemsSource = Enum.GetValues(typeof(Energy));
            TransmissionComboBox.ItemsSource = Enum.GetValues(typeof(TransmissionType));

            if (BodyTypeComboBox.Items.Count > 0)
                BodyTypeComboBox.SelectedIndex = -1;

            if (EnergyComboBox.Items.Count > 0)
                EnergyComboBox.SelectedIndex = -1;

            if (TransmissionComboBox.Items.Count > 0)
                TransmissionComboBox.SelectedIndex = -1;
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

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Listing listing)
            {
                var detailWindow = App.ServiceProvider.GetRequiredService<ListingDetailWindow>();
                detailWindow.Listing = listing;
                detailWindow.Show();
            }
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var query = SearchBox.Text;
            if (string.IsNullOrWhiteSpace(query))
            {
                LoadListings(); 
                MessageBox.Show("Please enter a search query", "Search Query", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            var result = await _listingService.SearchListings(query);
            ListingsItemsControl.ItemsSource = result;
        }

        private void TransmissionComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
        
        
        private void ApplyFilters()
        {
            var selectedTransmission = TransmissionComboBox.SelectedItem as TransmissionType?;
            var selectedBodyType = BodyTypeComboBox.SelectedItem as BodyType?;
            var selectedEnergy = EnergyComboBox.SelectedItem as Energy?;

            var filtered = AllListings.Where(l =>
                (!selectedTransmission.HasValue || l.TransmissionType == selectedTransmission.Value) &&
                (!selectedBodyType.HasValue || l.BodyType == selectedBodyType.Value) &&
                (!selectedEnergy.HasValue || l.Energy == selectedEnergy.Value)
            ).ToList();

            // Phải gán lại ItemsSource hoàn toàn
            ListingsItemsControl.ItemsSource = filtered;
        }

        private void BodyTypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();

        }
        

        private void EnergyComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
    }
}