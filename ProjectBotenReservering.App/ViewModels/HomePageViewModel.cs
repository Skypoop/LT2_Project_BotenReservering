using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.App.Views;

namespace ProjectBotenReservering.App.ViewModels;

public partial class HomePageViewModel : BaseViewModel
{
    [RelayCommand]
    public async Task SwapToBootTypesPage()
    {
        await Shell.Current.GoToAsync(nameof(BoatTypesView));   
    }
}