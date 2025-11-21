using System.Windows;
using System.Windows.Controls;
using ElectricVehicleManagement.Data.Models;
using ElectricVehicleManagement.Data.Models.Enums;
using ElectricVehicleManagement.Service.Listing;
using ElectricVehicleManagement.Service.User;
using Microsoft.Extensions.DependencyInjection;

namespace ElectricVehicleManagement.Presentation;

public partial class MainAdminWindow : Window
{
    private readonly IListingService _listingService;
    private readonly IUserService _userService;
    
    private bool _loadingPending = false;
    private bool _loadingHistory = false;
    private bool _loadingAccounts = false;
    
  
    private int _currentAccountPage = 1;
    private int _pageSize = 5;
    private int _totalAccountPages = 1;


    private string? _searchKeyword = null;


    public MainAdminWindow(IListingService listingService, IUserService userService)
    {
        _listingService = listingService;
        _userService = userService;
        InitializeComponent();
    }


    private async void MainAdminWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        await LoadPendingListings();
        await LoadHistoryListings();
        await LoadAccounts();
    }
    
    private async Task LoadAccounts()
    {
        
        if (_loadingAccounts) return;
        _loadingAccounts = true;

        try
        {
            AccountGrid.ItemsSource = null;
            AccountGrid.Items.Clear();

            var paged = await _userService.GetPaged(_currentAccountPage, _pageSize, _searchKeyword);

            AccountGrid.ItemsSource = paged.Items;

            _totalAccountPages = paged.TotalPages;
            AccountPageText.Text = $"Page {_currentAccountPage} / {_totalAccountPages}";

            PrevAccountBtn.IsEnabled = _currentAccountPage > 1;
            NextAccountBtn.IsEnabled = _currentAccountPage < _totalAccountPages;
        }
        finally
        {
            _loadingAccounts = false;
        }
    }

    private async void SearchAccountBtn_OnClick(object sender, RoutedEventArgs e)
    {
        _searchKeyword = SearchAccountBox.Text.Trim();

        _currentAccountPage = 1; 
        await LoadAccounts();
    }

    private async void PrevAccountBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (_currentAccountPage > 1)
        {
            _currentAccountPage--;
            await LoadAccounts();
        }
    }

    private async void NextAccountBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (_currentAccountPage < _totalAccountPages)
        {
            _currentAccountPage++;
            await LoadAccounts();
        }
    }
    
    private async Task LoadPendingListings()
    {
        if (_loadingPending) return;
        _loadingPending = true;

        try
        {
            var pendingListings = await _listingService.GetPendingListings();
            PendingListingGrid.ItemsSource = pendingListings;
        }
        finally
        {
            _loadingPending = false;
        }
    }

    private async Task LoadHistoryListings()
    {
        if (_loadingHistory) return;
        _loadingHistory = true;

        try
        {
            var historyListings = await _listingService.GetReviewedListings();
            HistoryListingGrid.ItemsSource = historyListings;
        }
        finally
        {
            _loadingHistory = false;
        }
    }
    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button == null) return;

        var listing = button.DataContext as Listing; 
        if (listing == null) return;

        var detailWindow = App.ServiceProvider.GetRequiredService<ListingDetailWindow>();
        detailWindow.Listing = listing;
        detailWindow.OnListingUpdated += async () => await LoadPendingListings();
        detailWindow.Show();
    }

    private async void HistoryFilterCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selected = (HistoryFilterCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
        var filtered = await _listingService.GetListingsByStatus(selected);
        HistoryListingGrid.ItemsSource = filtered;
    }

    private async void BanUnlockBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        if (button?.DataContext is not User user)
            return;

        if (user.Role == Role.Administrator)
        {
            MessageBox.Show("Can not block account admin", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }


        user.Status = !user.Status;
        await _userService.UpdateStatus(user.Id, user.Status);

        await LoadAccounts();
    }

    private void ReportBtn_Click_OnClick(object sender, RoutedEventArgs e)
    {
        var reportWindow = new DashboardWindow(_userService, _listingService);
        reportWindow.Show();
    }
}