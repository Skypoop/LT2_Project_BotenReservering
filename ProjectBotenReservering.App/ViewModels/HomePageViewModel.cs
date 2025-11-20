using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces;
using System.Text.Json;

namespace ProjectBotenReservering.App.ViewModels;

public partial class HomePageViewModel : BaseViewModel
{
    private readonly IWeatherService _weatherService;

    public HomePageViewModel(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [RelayCommand]
    public async Task GetWeather()
    {
        int windforce = await _weatherService.GetWeatherAsync();
        await Shell.Current.DisplayAlert("Weather Report", $"De windkracht is {windforce} met rang N is het mogelijk dat jouw reservering wordt geannuleerd", "OK");

    }
}