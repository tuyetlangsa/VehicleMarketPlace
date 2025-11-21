using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using ElectricVehicleManagement.Data.Models.Dto;
using ElectricVehicleManagement.Service.User;

namespace ElectricVehicleManagement.Presentation
{
    public partial class UserProfileWindow : Window
    {
        private readonly IUserService _userService;
        private readonly Guid _userId;
        private Data.Models.User _currentUser;
        private readonly HttpClient _http;

        public UserProfileWindow(Guid userId, IUserService userService)
        {
            InitializeComponent();
            _userId = userId;
            _userService = userService;
            _http = new HttpClient();

            LoadUserData();
            LoadAddressTree();
        }
        
        private async void LoadUserData()
        {
            _currentUser = await _userService.GetUserById(_userId);

            FullNameTextBox.Text = _currentUser.FullName;
            EmailTextBox.Text = _currentUser.Email;
            PhoneTextBox.Text = _currentUser.Phone;
            AddressTextBox.Text = _currentUser.Address;
        }
        
        private async void LoadAddressTree()
        {
            try
            {
                var provinces = await _http.GetFromJsonAsync<List<Province>>(
                    "https://provinces.open-api.vn/api/?depth=3");

                if (provinces == null) return;

                foreach (var p in provinces)
                {
                    var provinceNode = new TreeViewItem
                    {
                        Header = p.Name,
                        Tag = p.Code
                    };

                    foreach (var d in p.Districts ?? new List<District>())
                    {
                        var districtNode = new TreeViewItem
                        {
                            Header = d.Name,
                            Tag = d.Code
                        };

                        foreach (var w in d.Wards ?? new List<Ward>())
                        {
                            districtNode.Items.Add(new TreeViewItem
                            {
                                Header = w.Name,
                                Tag = w.Code
                            });
                        }

                        provinceNode.Items.Add(districtNode);
                    }

                    AddressTree.Items.Add(provinceNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading address API:\n" + ex.Message);
            }
        }
        
        private void AddressTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (AddressTree.SelectedItem is not TreeViewItem wardNode) return;
            
            if (wardNode.Parent is not TreeViewItem districtNode) return;
            if (districtNode.Parent is not TreeViewItem provinceNode) return;

            string ward = wardNode.Header.ToString();
            string district = districtNode.Header.ToString();
            string province = provinceNode.Header.ToString();

            AddressTextBox.Text = $"{ward}, {district}, {province}";
        }
        
        private bool ValidateForm()
        {
            if (!string.IsNullOrWhiteSpace(PhoneTextBox.Text) &&
                !PhoneTextBox.Text.All(char.IsDigit))
            {
                MessageBox.Show("Phone number must contain only digits.");
                return false;
            }

            return true;
        }
        
        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            
            if (!string.IsNullOrWhiteSpace(FullNameTextBox.Text))
                _currentUser.FullName = FullNameTextBox.Text.Trim();

            if (!string.IsNullOrWhiteSpace(PhoneTextBox.Text))
                _currentUser.Phone = PhoneTextBox.Text.Trim();

            if (!string.IsNullOrWhiteSpace(AddressTextBox.Text))
                _currentUser.Address = AddressTextBox.Text.Trim();

            await _userService.Update(_currentUser);

            MessageBox.Show("Profile updated successfully!", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);

            this.Close();
        }
    }
}
