using System.Globalization;

namespace EduPlayKids.App.Converters;

/// <summary>
/// Converter that returns true if the value is not null, false otherwise.
/// Useful for showing/hiding UI elements based on whether a property has a value.
/// </summary>
public class IsNotNullConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ConvertBack is not supported for IsNotNullConverter");
    }
}