using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ElectricVehicleManagement.Data.Models;
using ElectricVehicleManagement.Data.Models.Enums;
using ElectricVehicleManagement.Service.Listing;

namespace ElectricVehicleManagement.Presentation;

public partial class ListingDetailWindow : Window
{
    public  Listing Listing { get; set; }
    private readonly IListingService _listingService;
    public event Action OnListingUpdated;
    public ListingDetailWindow(IListingService listingService)
    {
        _listingService = listingService;
        InitializeComponent();
    }
    
    
    
    private void LoadImages()
    {
        if (Listing.Images == null || Listing.Images.Count == 0)
            return;

        ThumbnailList.ItemsSource = Listing.Images;

        var mainImg = Listing.Images.FirstOrDefault(i => i.IsPrimary)
                      ?? Listing.Images.First();

        MainImage.Source = new BitmapImage(new Uri(mainImg.ImageUrl));
    }

    private void Thumbnail_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is Image img && img.DataContext is ListingImage listingImage)
        {
            MainImage.Source = new BitmapImage(new Uri(listingImage.ImageUrl));
        }
    }

    private void ListingDetailWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (App.CurrentUser is null || App.CurrentUser.Role != Role.Administrator)
        {
            ApproveButton.Visibility = Visibility.Hidden;
            RejectButton.Visibility = Visibility.Hidden;
        }
        DataContext = Listing;
        LoadImages();    
    }


    private async void ButtonApprove_OnClick(object sender, RoutedEventArgs e)
    {
        var result = await _listingService.ApproveListing(Listing.ListingId);
        if (result)
        {
            OnListingUpdated?.Invoke();
            this.Close();
        }
    }

    private async void ButtonReject_OnClick(object sender, RoutedEventArgs e)
    {
        var result = await _listingService.RejectListing(Listing.ListingId);
        if (result)
        {
            OnListingUpdated?.Invoke();
            this.Close();
        }
    }
}