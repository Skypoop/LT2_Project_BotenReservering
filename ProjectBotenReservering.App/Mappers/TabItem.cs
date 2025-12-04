using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;

namespace ProjectBotenReservering.Core.Mappers;

public class TabItem
{
    public string ImageName { get; }
    public View Content { get; }

    public TabItem(string imageName, View content)
    {
        ImageName = imageName;
        Content = content;
    }
}

