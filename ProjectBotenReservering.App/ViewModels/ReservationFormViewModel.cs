using CommunityToolkit.Mvvm.Input;
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
        
        // standaard waardes voor kalander
        SelectedDate = DateTime.Today;
        StartTime = new TimeSpan(9, 0, 0);
        EndTime = new TimeSpan(10, 0, 0);
    }

    public DateTime MinDate => DateTime.Today;

    [ObservableProperty] 
    private BoatTypeUiItem _currentBoatType;

    [ObservableProperty]
    private DateTime _selectedDate;

    [ObservableProperty]
    private TimeSpan _startTime;

    [ObservableProperty]
    private TimeSpan _endTime;

    private int _boatId;
    public int BoatId
    {
        get => _boatId;
        set
        {
            if (SetProperty(ref _boatId, value))
            {
                // Fix: Ensure data loads when navigation happens
                LoadBoatDataCommand.Execute(value);
            }
        }
    }

    [RelayCommand]
    private async Task LoadBoatData(int id)
    {
        var boatType = _boatTypeService.GetBoatTypeById(id);
        CurrentBoatType = boatType;
    }

    [RelayCommand]
    private async Task SaveReservation()
    {
        if (EndTime <= StartTime)
        {
            await Shell.Current.DisplayAlert("Error", "End time must be after start time.", "OK");
            return;
        }

        DateTime startDateTime = SelectedDate.Date + StartTime;
        DateTime endDateTime = SelectedDate.Date + EndTime;

        await Shell.Current.DisplayAlert("Success", msg, "OK");
        await Shell.Current.GoToAsync("..");
    }
}