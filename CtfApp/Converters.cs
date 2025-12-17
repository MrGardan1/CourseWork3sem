using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace CtfApp;

public class BoolToBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isCurrentUser && isCurrentUser)
        {
            return SolidColorBrush.Parse("#313244");  // Подсветка для текущего юзера
        }
        return SolidColorBrush.Parse("#1e1e2e");  // Обычный фон
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BoolToFontWeightConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isCurrentUser && isCurrentUser)
        {
            return FontWeight.Bold;
        }
        return FontWeight.Normal;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class PositionToMedalConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int position && parameter is string targetPosition)
        {
            return position == int.Parse(targetPosition);
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
