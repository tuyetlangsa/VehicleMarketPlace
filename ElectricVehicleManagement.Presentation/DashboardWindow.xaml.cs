using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ElectricVehicleManagement.Service.User;
using ElectricVehicleManagement.Service.Listing;
using ElectricVehicleManagement.Data.Models.Enums;
using LiveCharts;
using LiveCharts.Wpf;

namespace ElectricVehicleManagement.Presentation
{
    public partial class DashboardWindow : Window
    {
        private readonly IUserService _userService;
        private readonly IListingService _listingService;

        public DashboardWindow(IUserService userService, IListingService listingService)
        {
            InitializeComponent();
            _userService = userService;
            _listingService = listingService;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUserStats();
            await LoadListingStats(null, null);
        }

        private async Task LoadUserStats()
        {
            var users = await _userService.GetAllUsers();
            TotalUsersText.Text = users.Count.ToString();
            ActiveUsersText.Text = users.Count(x => x.Status).ToString();
            BannedUsersText.Text = users.Count(x => !x.Status).ToString();
        }

        private async Task LoadListingStats(DateTime? from, DateTime? to)
        {
            var listings = await _listingService.GetAllListingsRaw();

            if (from != null && to != null)
                listings = listings.Where(x => x.CreatedAt >= from && x.CreatedAt <= to).ToList();

            int total = listings.Count;
            int pending = listings.Count(x => x.Status == ListingStatus.Pending);
            int approved = listings.Count(x => x.Status == ListingStatus.Approved);
            int rejected = listings.Count(x => x.Status == ListingStatus.Reject); // ✅ NEW
            
            TotalListingsText.Text = total.ToString();
            PendingListingsText.Text = pending.ToString();
            ApprovedListingsText.Text = approved.ToString();

            
            ListingsChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total",
                    Values = new ChartValues<int> { total },
                    Fill = System.Windows.Media.Brushes.MediumPurple,
                    StrokeThickness = 0,
                    DataLabels = true
                },
                new ColumnSeries
                {
                    Title = "Pending",
                    Values = new ChartValues<int> { pending },
                    Fill = System.Windows.Media.Brushes.DarkOrange,
                    StrokeThickness = 0,
                    DataLabels = true
                },
                new ColumnSeries
                {
                    Title = "Approved",
                    Values = new ChartValues<int> { approved },
                    Fill = System.Windows.Media.Brushes.SeaGreen,
                    StrokeThickness = 0,
                    DataLabels = true
                },
                new ColumnSeries
                {
                    Title = "Rejected",
                    Values = new ChartValues<int> { rejected },
                    Fill = System.Windows.Media.Brushes.IndianRed, // 🔴 ĐỎ – Reject
                    StrokeThickness = 0,
                    DataLabels = true
                }
            };
            
            ListingsChart.AxisX.Clear();
            ListingsChart.AxisX.Add(new Axis
            {
                Labels = new[] { "Listings" },
                Separator = new Separator { Step = 1 }
            });

            ListingsChart.AxisY.Clear();
            ListingsChart.AxisY.Add(new Axis
            {
                MinValue = 0,
                Title = "Count"
            });
        }


        private async void FilterListingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FromDatePicker.SelectedDate == null || ToDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please select both dates.");
                return;
            }

            await LoadListingStats(FromDatePicker.SelectedDate, ToDatePicker.SelectedDate);
        }
    }
}