using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using ElectricVehicleManagement.Data.Models;
using ElectricVehicleManagement.Data.Models.Enums;
using ElectricVehicleManagement.Service.Cloudinary;
using ElectricVehicleManagement.Service.Listing;
using Microsoft.Win32;

namespace ElectricVehicleManagement.Presentation
{
    public partial class PostListingWindow : Window
    {
        private readonly IListingService _listingService;
        private readonly ICloudinaryService _cloudinaryService;

   
        private readonly List<string> _selectedImagePaths = new();

        public PostListingWindow(
            IListingService listingService,
            ICloudinaryService cloudinaryService)
        {
            InitializeComponent();
            _listingService = listingService;
            _cloudinaryService = cloudinaryService;

            InitEnumCombos();
        }

        private void InitEnumCombos()
        {
            BodyTypeComboBox.ItemsSource = Enum.GetValues(typeof(BodyType));
            EnergyComboBox.ItemsSource = Enum.GetValues(typeof(Energy));
            TransmissionComboBox.ItemsSource = Enum.GetValues(typeof(TransmissionType));

            if (BodyTypeComboBox.Items.Count > 0)
                BodyTypeComboBox.SelectedIndex = 0;

            if (EnergyComboBox.Items.Count > 0)
                EnergyComboBox.SelectedIndex = 0;

            if (TransmissionComboBox.Items.Count > 0)
                TransmissionComboBox.SelectedIndex = 0;
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Choose images (3–10 files)",
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
            {
                int count = dialog.FileNames.Length;

                if (count < 3 || count > 10)
                {
                    MessageBox.Show(
                        "Please select at least 3 images and at most 10 images.",
                        "Invalid image count",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                _selectedImagePaths.Clear();
                _selectedImagePaths.AddRange(dialog.FileNames);

                SelectedImageTextBlock.Text = string.Join(", ",
                    _selectedImagePaths.Select(Path.GetFileName));

                SelectedImageCountTextBlock.Text =
                    $"{_selectedImagePaths.Count}/10 images selected";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
            }
            catch
            {
                // ignore if DialogResult cannot be set
            }

            Close();
        }

        private async void PostButton_Click(object sender, RoutedEventArgs e)
        {
            var currentUser = App.CurrentUser as User;
            if (currentUser == null)
            {
                MessageBox.Show(
                    "You must be logged in to post a listing.",
                    "Not authenticated",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!TryBuildBaseListing(currentUser, out var listing))
            {
         
                return;
            }

            try
            {
                var now = DateTime.UtcNow;
                var uploadedUrls = new List<string>();

                foreach (var path in _selectedImagePaths)
                {
                    var url = await _cloudinaryService.UploadFileAsync(path);
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        MessageBox.Show(
                            $"Cannot upload image: {Path.GetFileName(path)}",
                            "Upload error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    uploadedUrls.Add(url);
                }

      
                for (int i = 0; i < uploadedUrls.Count; i++)
                {
                    listing.Images.Add(new ListingImage
                    {
                        Id = Guid.NewGuid(),
                        ListingId = listing.ListingId,
                        ImageUrl = uploadedUrls[i],
                        IsPrimary = i == 0, 
                        UploadedAt = now
                    });
                }

          
                await _listingService.CreateListing(listing);  

                MessageBox.Show(
                    "Listing posted successfully.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error while saving listing: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

       
        private bool TryBuildBaseListing(User currentUser, out Listing listing)
        {
            listing = null!;
            var errors = new List<string>();

      
            string title = TitleTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(title))
                errors.Add("Title is required.");

            string description = DescriptionTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(description))
                errors.Add("Description is required.");

            string brand = BrandTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(brand))
                errors.Add("Brand is required.");

            string model = ModelTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(model))
                errors.Add("Model is required.");

   
            if (!decimal.TryParse(PriceTextBox.Text.Trim(), out var price) || price <= 0)
                errors.Add("Price must be a positive number.");

            var currentYear = DateTime.UtcNow.Year;
            if (!int.TryParse(YearTextBox.Text.Trim(), out var year) ||
                year < 1900 || year > currentYear)
                errors.Add($"Manufacturing year must be between 1900 and {currentYear}.");

     
            if (!int.TryParse(SeatingTextBox.Text.Trim(), out var seats) || seats <= 0)
                errors.Add("Seats must be a positive integer.");

      
            int? mileage = null;
            if (!string.IsNullOrWhiteSpace(MileageTextBox.Text))
            {
                if (!int.TryParse(MileageTextBox.Text.Trim(), out var mileageValue) || mileageValue < 0)
                    errors.Add("Mileage must be a non-negative integer.");
                else
                    mileage = mileageValue;
            }

            int? batteryCapacity = null;
            if (!string.IsNullOrWhiteSpace(BatteryCapTextBox.Text))
            {
                if (!int.TryParse(BatteryCapTextBox.Text.Trim(), out var batteryCapValue) || batteryCapValue <= 0)
                    errors.Add("Battery capacity (kWh) must be a positive integer.");
                else
                    batteryCapacity = batteryCapValue;
            }

 
            decimal? batteryHealth = null;
            if (!string.IsNullOrWhiteSpace(BatteryCondTextBox.Text))
            {
                if (!decimal.TryParse(BatteryCondTextBox.Text.Trim(), out var health) ||
                    health < 0 || health > 100)
                    errors.Add("Battery health (%) must be a number between 0 and 100.");
                else
                    batteryHealth = health;
            }


            BodyType bodyType = default;
            if (BodyTypeComboBox.SelectedItem is BodyType bt)
            {
                bodyType = bt;
            }
            else
            {
                errors.Add("Please choose a body type.");
            }

            Energy energy = default;
            if (EnergyComboBox.SelectedItem is Energy en)
            {
                energy = en;
            }
            else
            {
                errors.Add("Please choose energy type.");
            }

            TransmissionType transmission = default;
            if (TransmissionComboBox.SelectedItem is TransmissionType tr)
            {
                transmission = tr;
            }
            else
            {
                errors.Add("Please choose transmission type.");
            }

            if (_selectedImagePaths.Count < 3 || _selectedImagePaths.Count > 10)
                errors.Add("Please select between 3 and 10 images.");

            if (errors.Any())
            {
                MessageBox.Show(
                    string.Join(Environment.NewLine, errors),
                    "Invalid data",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            var now = DateTime.UtcNow;

            listing = new Listing
            {
                ListingId = Guid.NewGuid(),
                UserId = currentUser.Id,
                Title = title,
                Description = description,
                Price = price,
                BatteryCapacityKwh = batteryCapacity,
                BatteryConditionPercent = batteryHealth,
                MileageKm = mileage,
                ManufacturingYear = year,
                VehicleBrand = brand,
                VehicleModel = model,
                TransmissionType = transmission,
                SeatingCapacity = seats,
                BodyType = bodyType,
                Energy = energy,
                Location = string.IsNullOrWhiteSpace(LocationTextBox.Text)
                    ? null
                    : LocationTextBox.Text.Trim(),
                Status = ListingStatus.Pending,
                IsVisible = true,
                CreatedAt = now,
                UpdatedAt = null,
                Images = new List<ListingImage>()
            };

            return true;
        }
    }
}
