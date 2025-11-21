using System.Windows;
using System.Windows.Controls;
using ElectricVehicleManagement.Data.Models;
using ElectricVehicleManagement.Service.Listing;
using Microsoft.Extensions.DependencyInjection;

namespace ElectricVehicleManagement.Presentation;

public partial class MainAdminWindow : Window
{
    private readonly IListingService _listingService;

    public MainAdminWindow(IListingService listingService)
    {
        _listingService = listingService;
        InitializeComponent();
    }


    private async void MainAdminWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        await LoadPendingListings();
        await LoadHistoryListings();
    }
    
    private async Task LoadPendingListings()
    {
        var pendingListings = await _listingService.GetPendingListings();
        PendingListingGrid.ItemsSource = pendingListings ;
    }

    private async Task LoadHistoryListings()
    {
        var historyListings = await _listingService.GetReviewedListings();
        HistoryListingGrid.ItemsSource = historyListings;
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
}