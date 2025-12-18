using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class BoatTypeSelectionCompetitionViewModel : BaseViewModel, IQueryAttributable
{
    private DateTime _startDateTime = DateTime.MinValue;
    private DateTime _endDateTime = DateTime.MinValue;
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query == null) return;
        if (query.TryGetValue("start", out object? startObj) && startObj is DateTime start)
            _startDateTime = start;
        if (query.TryGetValue("end", out object? endObj) && endObj is DateTime end)
            _endDateTime = end;
    }
    
    public ObservableCollection<BoatTypeUiItem> BoatTypeItems { get; set; } = [];
    public List<BoatTypeUiItem> AllBoatTypes { get; set; }

    public IReadOnlyList<SteeringOption> SteeringOptions { get; } = new List<SteeringOption>
    {
        new("Alles", null),
        new("Met Stuur", true),
        new("Zonder Stuur", false)
    };

    [ObservableProperty]
    public partial SteeringOption SelectedSteeringOption { get; set; }

    [ObservableProperty]
    public partial string StringInNameFilter { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int MinWeightFilter { get; set; } = 0;

    private readonly IBoatTypeService _boatTypeService;
    private readonly ICompetitionService _competitionService;

    public BoatTypeSelectionCompetitionViewModel(IBoatTypeService boatTypeService, ICompetitionService competitionService)
    {
        _boatTypeService = boatTypeService;
        AllBoatTypes = _boatTypeService.GetBoatTypes();
        SelectedSteeringOption = SteeringOptions.First();
        _competitionService = competitionService;

        ApplyFilterOption();
    }

    public async Task OnAppearing()
    {
        await ValidateBoatTypeList();
    }

    private async Task ValidateBoatTypeList()
    {
        if (AllBoatTypes.Count == 0)
            await Shell.Current.DisplayAlert(
                "Alert",
                "Geen boten binnen client rang gevonden. Neem contact op met de beheerder.",
                "OK");
    }

    private void ApplyFilterOption()
    {
        bool? steeringValue = SelectedSteeringOption?.Value;
        List<BoatTypeUiItem> boatTypeList = _boatTypeService.FilterBoatTypes(
            AllBoatTypes,
            steeringValue,
            StringInNameFilter,
            MinWeightFilter);

        BoatTypeItems.Clear();
        List<BoatTypeUiItem> orderedBoatTypeList = boatTypeList.OrderBy(x => x.Weight).ToList();

        foreach (BoatTypeUiItem boatType in orderedBoatTypeList)
        {
            BoatTypeItems.Add(boatType);
        }
    }

    [RelayCommand]
    private async Task Back()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task SelectBoatType(BoatTypeUiItem boatUiItem)
    {
        BoatTypeUiItem boat = _boatTypeService.GetBoatTypeById(boatUiItem.Id);

        if (_competitionService.HasEnoughBoats(boat.Id) == false)
        {
            await Shell.Current.DisplayAlert(
                "Niet genoeg boten",
                $"Er zijn niet genoeg boten van het type {boat.Name} beschikbaar.",
                "OK");
            return;
        }
        
        bool hasOverlap = _competitionService.SetSelectedBoat(boat.Id, _startDateTime, _endDateTime);
        if (hasOverlap)
        {
            bool shouldContinue = await ShowOverlapWarning();
            
            if (!shouldContinue)
                return;
        }

        await Shell.Current.GoToAsync("..");
    }
    
    private async Task<bool> ShowOverlapWarning()
    {
        return await Shell.Current.DisplayAlert(
            "Waarschuwing",
            "Er zijn te weinig boten beschikbaar voor de geselecteerde boottype en tijdsperiode. U kunt doorgaan, dan worden de overlappende reserveringen geannuleerd bij het bevestigen van de wedstrijd.",
            "Ga door",
            "Ga terug");
    }

    partial void OnSelectedSteeringOptionChanged(SteeringOption value) => ApplyFilterOption();
    partial void OnStringInNameFilterChanged(string value) => ApplyFilterOption();
    partial void OnMinWeightFilterChanged(int value) => ApplyFilterOption();
}