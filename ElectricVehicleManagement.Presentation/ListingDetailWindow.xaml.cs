using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ElectricVehicleManagement.Data.Models;

namespace ElectricVehicleManagement.Presentation;

public partial class ListingDetailWindow : Window
{
    public  Listing Listing { get; set; }

    public ListingDetailWindow()
    {
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
        DataContext = Listing;
        LoadImages();    
    }
}