using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.App.Views;

namespace ProjectBotenReservering.App.ViewModels;

public partial class BoatTypesViewModel : BaseViewModel
{
    public ObservableCollection<BoatTypeUiItem> BoatTypeItems { get; set; } = [];
    public List<BoatTypeUiItem> AllBoatTypes { get; set; }
    public IReadOnlyList<SteeringOption> SteeringOptions { get; } = new List<SteeringOption>
    {
        new("Alles", null),
        new("Met Stuur", true),
        new("Zonder Stuur", false)
    };

    [ObservableProperty]
    public SteeringOption selectedSteeringOption;
    
    [ObservableProperty]
    public string stringInNameFilter = String.Empty;
    
    [ObservableProperty]
    public int minWeightFilter = 0;
    
    private readonly IBoatTypeService _boatTypeService;
    
    public BoatTypesViewModel(IBoatTypeService boatTypeService)
    {
        _boatTypeService = boatTypeService;
        AllBoatTypes = _boatTypeService.GetBoatTypes();
        SelectedSteeringOption = SteeringOptions.First();
        ApplyFilterOption();
    }

    private void ApplyFilterOption()
    {
        bool? steeringValue = SelectedSteeringOption?.Value;

        List<BoatTypeUiItem> boatTypeList = _boatTypeService.FilterBoatTypes(AllBoatTypes, steeringValue, StringInNameFilter, MinWeightFilter);
        BoatTypeItems.Clear();
        
        List<BoatTypeUiItem> orderedBoatTypeList = boatTypeList.OrderBy(x => x.Weight).ToList();
        foreach (var boatType in orderedBoatTypeList)
        {
            BoatTypeItems.Add(boatType);
        }
    }
    
    // Select a boat type
    [RelayCommand]
    public async Task SelectBoatType(BoatTypeUiItem boatType)
    {
        string route = $"{nameof(ReservationFormView)}?Id={boatType.Id}";
        await Shell.Current.GoToAsync(route);
    }

    // Filter callbacks
    partial void OnSelectedSteeringOptionChanged(SteeringOption value) => ApplyFilterOption(); 
    partial void OnStringInNameFilterChanged(string value) => ApplyFilterOption();
    partial void OnMinWeightFilterChanged(int value) => ApplyFilterOption();

}
