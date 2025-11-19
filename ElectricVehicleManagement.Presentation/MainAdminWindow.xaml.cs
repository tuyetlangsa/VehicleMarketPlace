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
    }
    
    private async Task LoadPendingListings()
    {
        var pendingListings = await _listingService.GetPendingListings();
        PendingListingGrid.ItemsSource = pendingListings ;
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
}