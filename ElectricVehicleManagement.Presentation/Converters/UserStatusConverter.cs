using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ElectricVehicleManagement.Presentation.Converters;

public class UserStatusConverter : IValueConverter
{
    // parameter = "text" hoặc "color"
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isActive = (bool)value; // Status
        string mode = parameter?.ToString()?.ToLower() ?? "text";

        if (mode == "text")
        {
            return isActive ? "Lock" : "Unlock";
        }

        if (mode == "color")
        {
            return isActive
                ? new SolidColorBrush(Color.FromRgb(192, 57, 43))
                : new SolidColorBrush(Color.FromRgb(39, 174, 96)); 
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}