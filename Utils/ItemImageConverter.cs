using System.Globalization;

namespace AstroBoy.Utils;

public class ItemImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string id && !string.IsNullOrEmpty(id))
        {
            return ImageSource.FromStream(async (ct) =>
            {
                try
                {
                    return await FileSystem.OpenAppPackageFileAsync($"items/{id}.jpg");
                }
                catch
                {
                    return null;
                }
            });
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
