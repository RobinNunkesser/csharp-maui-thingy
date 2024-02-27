using System.Globalization;

namespace Thingy;

public class UuidConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        var uuid = (value as string)?.ToUpper();
        ThingyUUIDs.Uuid.TryGetValue(uuid, out var service);
        return service ?? value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter,
        CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}