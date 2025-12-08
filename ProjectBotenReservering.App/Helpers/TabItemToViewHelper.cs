using System.Globalization;
using ProjectBotenReservering.Core.Models; 

namespace ProjectBotenReservering.App.Helpers;

public class TabItemToViewHelper : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not TabItem tabItem)
        {
            return null;
        }

        Type viewType = tabItem.ContentType;

        View view = (View)App.ServiceProvider!.GetRequiredService(viewType);

        return view;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}