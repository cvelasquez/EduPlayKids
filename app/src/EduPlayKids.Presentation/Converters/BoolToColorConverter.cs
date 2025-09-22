using System.Globalization;

namespace EduPlayKids.App.Converters;

/// <summary>
/// Converter for mapping boolean values to colors.
/// Useful for selection states and conditional styling.
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to a color.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    /// <param name="targetType">The target type (Color).</param>
    /// <param name="parameter">Optional parameter for custom colors.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>Color based on boolean value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            // Use parameter for custom colors if provided
            if (parameter is string colorPair)
            {
                var colors = colorPair.Split(',');
                if (colors.Length == 2)
                {
                    return boolValue ? Color.FromArgb(colors[0].Trim()) : Color.FromArgb(colors[1].Trim());
                }
            }

            // Default colors for selection states
            return boolValue ? Color.FromArgb("#E3F2FD") : Color.FromArgb("#F8F9FA");
        }

        return Color.FromArgb("#F8F9FA");
    }

    /// <summary>
    /// Converts back from color to boolean (not implemented).
    /// </summary>
    /// <param name="value">The color value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>Not implemented - throws NotImplementedException.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("BoolToColorConverter.ConvertBack is not implemented.");
    }
}