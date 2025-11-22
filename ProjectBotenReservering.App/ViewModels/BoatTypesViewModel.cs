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

    [ObservableProperty]
    public bool hasSteeringWheelFilter = false;
    
    [ObservableProperty]
    public string stringInNameFilter = String.Empty;
    
    [ObservableProperty]
    public int minWeightFilter = 0;
    
    private readonly IBoatTypeService BoatTypeService;
    
    public BoatTypesViewModel(IBoatTypeService boatTypeService)
    {
        BoatTypeService = boatTypeService;

        AllBoatTypes = BoatTypeService.GetBoatTypes();
        
        ApplyFilterOption();
    }

    private void ApplyFilterOption()
    {
        List<BoatTypeUiItem> boatTypeList = BoatTypeService.FilterBoatTypes(AllBoatTypes, HasSteeringWheelFilter, StringInNameFilter, MinWeightFilter);
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
    partial void OnHasSteeringWheelFilterChanged(bool value) => ApplyFilterOption();
    partial void OnStringInNameFilterChanged(string value) => ApplyFilterOption();
    partial void OnMinWeightFilterChanged(int value) => ApplyFilterOption();

}
