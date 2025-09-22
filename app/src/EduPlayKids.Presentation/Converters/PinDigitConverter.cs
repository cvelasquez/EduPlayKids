using System.Globalization;

namespace EduPlayKids.App.Converters;

/// <summary>
/// Converter for extracting individual PIN digits for UI display.
/// Supports secure PIN entry with individual digit fields.
/// </summary>
public class PinDigitConverter : IValueConverter
{
    /// <summary>
    /// Converts a PIN string to an individual digit at the specified position.
    /// </summary>
    /// <param name="value">The complete PIN string.</param>
    /// <param name="targetType">The target type (string).</param>
    /// <param name="parameter">The digit position (0-3).</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>The digit at the specified position or empty string.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string pin || parameter is not string positionStr)
            return string.Empty;

        if (!int.TryParse(positionStr, out int position))
            return string.Empty;

        if (position < 0 || position >= 4)
            return string.Empty;

        if (string.IsNullOrEmpty(pin) || position >= pin.Length)
            return string.Empty;

        return pin[position].ToString();
    }

    /// <summary>
    /// Converts back from individual digit to PIN string (not implemented).
    /// </summary>
    /// <param name="value">The digit value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">The digit position.</param>
    /// <param name="culture">The culture info.</param>
    /// <returns>Not implemented - throws NotImplementedException.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("PinDigitConverter.ConvertBack is not implemented. Use code-behind for PIN digit management.");
    }
}