using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class BoatTypesViewModel : BaseViewModel
{
    public ObservableCollection<BoatTypeUiItem> BoatTypeItems { get; set; } = new ObservableCollection<BoatTypeUiItem>();
    public List<BoatTypeUiItem> AllBoatTypes { get; set; }

    [ObservableProperty]
    public bool hasSteeringWheelFilter = false;
    
    [ObservableProperty]
    public string hasStringInNameFilter = String.Empty;
    
    [ObservableProperty]
    public int hasMinWeightFilter = 0;
    
    private readonly IBoatTypeService BoatTypeService;
    
    public BoatTypesViewModel(IBoatTypeService boatTypeService)
    {
        BoatTypeService = boatTypeService;

        AllBoatTypes = BoatTypeService.GetBoatTypes();
        
        ApplyFilterOption();
    }

    private void ApplyFilterOption()
    {
        List<BoatTypeUiItem> newList = BoatTypeService.FilterBoatTypes(AllBoatTypes, HasSteeringWheelFilter, HasStringInNameFilter, HasMinWeightFilter);
        BoatTypeItems.Clear();
        
        newList = newList.OrderBy(x => x.Weight).ToList();
        foreach (var boatType in newList)
        {
            BoatTypeItems.Add(boatType);
        }
    }
    
    // Select a boat type
    [RelayCommand]
    public void SelectBoatType(BoatTypeUiItem boatType)
    {
        Console.WriteLine(boatType.Name);
        // SET THE SERVICE VARIABLE FOR BOAT TYPE HERE
        // SWAP SCREEN
    }

    // Filter callbacks
    partial void OnHasSteeringWheelFilterChanged(bool value) => ApplyFilterOption();
    partial void OnHasStringInNameFilterChanged(string value) => ApplyFilterOption();
    partial void OnHasMinWeightFilterChanged(int value) => ApplyFilterOption();

}
