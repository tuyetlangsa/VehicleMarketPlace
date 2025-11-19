using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using ElectricVehicleManagement.Data.Models;
using ElectricVehicleManagement.Data.Models.Enums;
using ElectricVehicleManagement.Service.Cloudinary;
using ElectricVehicleManagement.Service.Listing;

namespace ElectricVehicleManagement.Presentation
{
    public partial class EditListingWindow : Window
    {
        private readonly IListingService _listingService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly Guid _listingId;
        private readonly List<Guid> _remainingImageIds = new();


        private Listing _originalListing;

        private readonly List<ListingImage> _existingImages = new();
        private readonly List<string> _newImagePaths = new();


        public EditListingWindow(
            IListingService listingService,
            ICloudinaryService cloudinaryService,
            Guid listingId)
        {
            InitializeComponent();

            _listingService = listingService;
            _cloudinaryService = cloudinaryService;
            _listingId = listingId;

            InitEnums();
            LoadListing();
        }

        private void InitEnums()
        {
            BodyTypeComboBox.ItemsSource = Enum.GetValues(typeof(BodyType));
            EnergyComboBox.ItemsSource = Enum.GetValues(typeof(Energy));
            TransmissionComboBox.ItemsSource = Enum.GetValues(typeof(TransmissionType));
        }

        private async void LoadListing()
        {
            _originalListing = await _listingService.GetListingById(_listingId);

            if (_originalListing == null)
            {
                MessageBox.Show("Listing not found.", "Error");
                Close();
                return;
            }
            
            _remainingImageIds.Clear();
            _remainingImageIds.AddRange(_originalListing.Images.Select(i => i.Id));

     
            TitleTextBox.Text = _originalListing.Title;
            DescriptionTextBox.Text = _originalListing.Description;
            PriceTextBox.Text = _originalListing.Price.ToString();
            YearTextBox.Text = _originalListing.ManufacturingYear.ToString();
            BrandTextBox.Text = _originalListing.VehicleBrand;
            ModelTextBox.Text = _originalListing.VehicleModel;
            SeatingTextBox.Text = _originalListing.SeatingCapacity.ToString();
            MileageTextBox.Text = _originalListing.MileageKm?.ToString() ?? "";
            BatteryCapTextBox.Text = _originalListing.BatteryCapacityKwh?.ToString() ?? "";
            BatteryCondTextBox.Text = _originalListing.BatteryConditionPercent?.ToString() ?? "";
            LocationTextBox.Text = _originalListing.Location;

            BodyTypeComboBox.SelectedItem = _originalListing.BodyType;
            EnergyComboBox.SelectedItem = _originalListing.Energy;
            TransmissionComboBox.SelectedItem = _originalListing.TransmissionType;

            _existingImages.Clear();
            _existingImages.AddRange(_originalListing.Images);
            RefreshExistingImagesUI();
        }

        private void RefreshExistingImagesUI()
        {
            ExistingImagesList.ItemsSource = null;
            ExistingImagesList.ItemsSource = _existingImages;
        }

        private void RemoveExistingImage_Click(object sender, RoutedEventArgs e)
        {
            var btn = (System.Windows.Controls.Button)sender;
            var id = (Guid)btn.Tag;

            var img = _existingImages.FirstOrDefault(i => i.Id == id);
            if (img != null)
            {
                _existingImages.Remove(img);
                _remainingImageIds.Remove(id);
            }

            RefreshExistingImagesUI();
        }


        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image files (*.jpg;*.png)|*.jpg;*.png"
            };

            if (dlg.ShowDialog() == true)
            {
                _newImagePaths.Clear();
                _newImagePaths.AddRange(dlg.FileNames);

                SelectedImageTextBlock.Text = string.Join(", ",
                    _newImagePaths.Select(System.IO.Path.GetFileName));

                SelectedImageCountTextBlock.Text =
                    $"{_newImagePaths.Count} new images selected";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
      
            if (!TryBuildEditedListing(out var updated))
                return;
            
            var newUrls = new List<string>();
            foreach (var path in _newImagePaths)
            {
                var url = await _cloudinaryService.UploadFileAsync(path);
                newUrls.Add(url);
            }

            await _listingService.UpdateListing(
                updated,
                _remainingImageIds,
                newUrls);

            MessageBox.Show("Listing updated successfully.",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }

        private bool TryBuildEditedListing(out Listing updated)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                errors.Add("Title is required.");

            if (!decimal.TryParse(PriceTextBox.Text, out var price) || price <= 0)
                errors.Add("Price must be a positive number.");

            if (!int.TryParse(YearTextBox.Text, out var year) || year < 1900)
                errors.Add("Invalid manufacturing year.");

            if (!int.TryParse(SeatingTextBox.Text, out var seats) || seats <= 0)
                errors.Add("Seats must be positive.");

            int totalImages = _remainingImageIds.Count + _newImagePaths.Count;
            if (totalImages < 3 || totalImages > 10)
                errors.Add("Total images must be between 3 and 10.");

            if (errors.Any())
            {
                MessageBox.Show(string.Join("\n", errors), "Invalid data");
                updated = null!;
                return false;
            }
            
            updated = new Listing
            {
                ListingId = _originalListing.ListingId,
                UserId = _originalListing.UserId,
                CreatedAt = _originalListing.CreatedAt,
                Status = ListingStatus.Pending,
                UpdatedAt = DateTime.UtcNow,
            };

            updated.Title = TitleTextBox.Text.Trim();
            updated.Description = DescriptionTextBox.Text.Trim();
            updated.Price = price;
            updated.ManufacturingYear = year;
            updated.VehicleBrand = BrandTextBox.Text.Trim();
            updated.VehicleModel = ModelTextBox.Text.Trim();
            updated.SeatingCapacity = seats;

            updated.MileageKm = string.IsNullOrWhiteSpace(MileageTextBox.Text)
                ? null
                : int.Parse(MileageTextBox.Text);

            updated.BatteryCapacityKwh = string.IsNullOrWhiteSpace(BatteryCapTextBox.Text)
                ? null
                : int.Parse(BatteryCapTextBox.Text);

            updated.BatteryConditionPercent = string.IsNullOrWhiteSpace(BatteryCondTextBox.Text)
                ? null
                : decimal.Parse(BatteryCondTextBox.Text);

            updated.BodyType = (BodyType)BodyTypeComboBox.SelectedItem;
            updated.Energy = (Energy)EnergyComboBox.SelectedItem;
            updated.TransmissionType = (TransmissionType)TransmissionComboBox.SelectedItem;

            updated.Location = string.IsNullOrWhiteSpace(LocationTextBox.Text)
                ? null
                : LocationTextBox.Text.Trim();

            return true;
        }
    }
}