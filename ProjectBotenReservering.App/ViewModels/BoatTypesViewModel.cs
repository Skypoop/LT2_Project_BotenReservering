using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class BoatTypesViewModel : BaseViewModel
{
    public ObservableCollection<BoatType> BoatTypes { get; set; } = new ObservableCollection<BoatType>();
    public List<BoatType> AllBoatTypes { get; set; } = new List<BoatType>();

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
        
        AllBoatTypes.Add(new BoatType() { Id = BoatTypes.Count, Name = "Skiff", Weight = 45.3f, ImagePath = "dotnet_bot.png"});
        AllBoatTypes.Add(new BoatType() { Id = BoatTypes.Count, Name = "Dubbel twee", Weight = 45.7f, ImagePath = "dotnet_bot.png"});
        AllBoatTypes.Add(new BoatType() { Id = BoatTypes.Count, Name = "Twee zonder", Weight = 46.2f, ImagePath = "dotnet_bot.png"});
        AllBoatTypes.Add(new BoatType() { Id = BoatTypes.Count, Name = "Twee met", Weight = 47.5f, ImagePath = "dotnet_bot.png"});
        AllBoatTypes.Add(new BoatType() { Id = BoatTypes.Count, Name = "Dubbel vier", Weight = 48.27f, ImagePath = "dotnet_bot.png"});
        AllBoatTypes.Add(new BoatType() { Id = BoatTypes.Count, Name = "Dubbel vier met", Weight = 49.92f, ImagePath = "dotnet_bot.png"});
        AllBoatTypes.Add(new BoatType() { Id = BoatTypes.Count, Name = "Vier zonder", Weight = 52.27f, ImagePath = "dotnet_bot.png"});
        AllBoatTypes.Add(new BoatType() { Id = BoatTypes.Count, Name = "Vier zonder met", Weight = 53.27f, ImagePath = "dotnet_bot.png"});
        AllBoatTypes.Add(new BoatType() { Id = BoatTypes.Count, Name = "Acht", Weight = 55.27f, ImagePath = "dotnet_bot.png"});
        
        ApplyFilterOption();
    }

    private void ApplyFilterOption()
    {
        List<BoatType> newList = BoatTypeService.FilterBoatTypes(AllBoatTypes, HasSteeringWheelFilter, HasStringInNameFilter, HasMinWeightFilter);
        BoatTypes.Clear();
        
        newList = newList.OrderBy(x => x.Weight).ToList();
        foreach (var boatType in newList)
        {
            BoatTypes.Add(boatType);
        }
    }
    
    // Select a boat type
    [RelayCommand]
    public void SelectBoatType(BoatType boatType)
    {
        Console.WriteLine(boatType.Name);
    }

    // Filter callbacks
    partial void OnHasSteeringWheelFilterChanged(bool value) => ApplyFilterOption();
    partial void OnHasStringInNameFilterChanged(string value) => ApplyFilterOption();
    partial void OnHasMinWeightFilterChanged(int value) => ApplyFilterOption();

}
