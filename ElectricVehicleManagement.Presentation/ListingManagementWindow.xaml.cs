using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ElectricVehicleManagement.Data.Models.Dto;
using ElectricVehicleManagement.Service.Cloudinary;
using ElectricVehicleManagement.Service.Listing;

namespace ElectricVehicleManagement.Presentation
{
    public partial class ListingManagementWindow : Window
    {
        private readonly IListingService _listingService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly Guid _currentUserId;
        private bool _isUpdatingVisibility = false;


        private int _page = 1;
        private const int PageSize = 5;

        private string _keyword = "";

        public ListingManagementWindow(IListingService listingService, ICloudinaryService cloudinaryService, Guid userId)
        {
            InitializeComponent();
            _listingService = listingService;
            _cloudinaryService = cloudinaryService;
            _currentUserId = userId;

            _ = LoadListings();
        }


        private async Task LoadListings()
        {
            var result = await _listingService.GetListingsByUser(
                _currentUserId,
                _page,
                PageSize,
                _keyword
            );

            ListingDataGrid.ItemsSource = result.Items.Select(l => new UserListingViewModel
            {
                ListingId = l.ListingId,
                Title = l.Title,
                Price = l.Price,
                Status = l.Status,
                IsVisible = l.IsVisible,
                PrimaryImageUrl = l.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
            }).ToList();

            PageLabel.Text = $"Page {result.Page}/{result.TotalPages}";
            PrevButton.IsEnabled = result.Page > 1;
            NextButton.IsEnabled = result.Page < result.TotalPages;
        }


        
        private async void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (_page > 1)
            {
                _page--;
                await LoadListings();
            }
        }

        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            _page++;
            await LoadListings();
        }


        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Placeholder.Visibility = string.IsNullOrWhiteSpace(SearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }


        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            _keyword = SearchTextBox.Text.Trim();
            _page = 1; 
            await LoadListings();
        }

        
        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            Placeholder.Visibility = Visibility.Visible;

            _keyword = "";
            _page = 1;

            await LoadListings();
        }

        
        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var listingId = (Guid)button.Tag;

            var editWindow = new EditListingWindow(
                _listingService,
                _cloudinaryService,
                listingId
            );

            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
                await LoadListings();
        }

        
        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var listingId = (Guid)button.Tag;

            var confirm = MessageBox.Show(
                "Are you sure you want to delete this listing?",
                "Confirm delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            await _listingService.DeleteListing(listingId);
            await LoadListings();
        }

        
        private async void VisibilityCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingVisibility) return;
            _isUpdatingVisibility = true;

            var checkbox = (CheckBox)sender;
            var vm = (UserListingViewModel)checkbox.DataContext;

            try
            {
                await _listingService.UpdateVisibility(vm.ListingId, checkbox.IsChecked == true);
            }
            finally
            {
                _isUpdatingVisibility = false;
            }
        }
        
    }
}
