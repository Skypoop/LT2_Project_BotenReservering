using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

[QueryProperty(nameof(BoatId), "Id")]
public partial class ReservationFormViewModel : BaseViewModel
{
    private readonly IBoatTypeService _boatTypeService;

    public ReservationFormViewModel(IBoatTypeService boatTypeService)
    {
        _boatTypeService = boatTypeService;
    }

    [ObservableProperty] 
    private BoatTypeUiItem _currentBoatType;

    private int _boatId;

    public int BoatId
    {
        get => _boatId;
        set
        {
            if (SetProperty(ref _boatId, value))
            {
            }
            

        }
    }

    [RelayCommand]
    private async Task LoadBoatData(int id)
    {
        var boatType = _boatTypeService.GetBoatTypeById(id);
        CurrentBoatType = boatType;
        Task.Delay(100);
    }
}
