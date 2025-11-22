using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class BoatTypesViewModel : BaseViewModel
{
    public ObservableCollection<BoatTypeUiItem> BoatTypeItems { get; set; } = [];
    public List<BoatTypeUiItem> AllBoatTypes { get; set; }

    public class SteeringOption
    {
        public required string DisplayName { get; set; }
        public bool? Value { get; set; }
        //NULL = all, TRUE = With Steering, FALSE = Without Steering
    }
    public List<SteeringOption> SteeringOptions { get; } = new List<SteeringOption> 
    { 
    new SteeringOption {DisplayName = "Alles", Value = null},
    new SteeringOption {DisplayName = "Met Stuur ", Value = true},
    new SteeringOption {DisplayName = "Zonder Stuur", Value = false}
    };

    [ObservableProperty]
    public SteeringOption selectedSteeringOption;
    
    [ObservableProperty]
    public string stringInNameFilter = String.Empty;
    
    [ObservableProperty]
    public int minWeightFilter = 0;
    
    private readonly IBoatTypeService BoatTypeService;
    

    public BoatTypesViewModel(IBoatTypeService boatTypeService)
    {
        BoatTypeService = boatTypeService;
        AllBoatTypes = BoatTypeService.GetBoatTypes();
        SelectedSteeringOption = SteeringOptions.First();
        ApplyFilterOption();
    }

    private void ApplyFilterOption()
    {
        bool? steeringValue = SelectedSteeringOption?.Value;

        List<BoatTypeUiItem> boatTypeList = BoatTypeService.FilterBoatTypes(AllBoatTypes, steeringValue, StringInNameFilter, MinWeightFilter);
        BoatTypeItems.Clear();
        
        List<BoatTypeUiItem> orderedBoatTypeList = boatTypeList.OrderBy(x => x.Weight).ToList();
        foreach (var boatType in orderedBoatTypeList)
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
    partial void OnSelectedSteeringOptionChanged(SteeringOption value) => ApplyFilterOption(); 
    partial void OnStringInNameFilterChanged(string value) => ApplyFilterOption();
    partial void OnMinWeightFilterChanged(int value) => ApplyFilterOption();

}
