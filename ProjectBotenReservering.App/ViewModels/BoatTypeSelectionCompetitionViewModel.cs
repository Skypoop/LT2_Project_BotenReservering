using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Exceptions;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class BoatTypeSelectionCompetitionViewModel : BaseViewModel
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
        AllBoatTypes = _boatTypeService.GetAllBoatTypes();
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
        BoatTypeUiItem? boat = GetBoatTypeOrNull(boatUiItem);
        if (boat is null) return;

        if (!TrySetSelectedBoat(boat.Id, out NotEnoughBoatsException? ex))
        {
            await ShowNotEnoughBoatsPopupAsync(ex!);
            return;
        }

        await NavigateBackAsync();
    }

    private BoatTypeUiItem? GetBoatTypeOrNull(BoatTypeUiItem boatUiItem)
    => _boatTypeService.GetBoatTypeById(boatUiItem.Id);

    private bool TrySetSelectedBoat(int boatId, out NotEnoughBoatsException? ex)
    {
        try
        {
            _competitionService.SelectedBoatId = boatId;
            ex = null;
            return true;
        }
        catch (NotEnoughBoatsException e)
        {
            ex = e;
            return false;
        }
    }

    private static Task ShowNotEnoughBoatsPopupAsync(NotEnoughBoatsException ex)
    => Shell.Current.DisplayAlert(
        "Te weinig boten",
        $"Nodig: {ex.Needed}\nBeschikbaar: {ex.Available}",
        "OK");

    private static Task NavigateBackAsync()
        => Shell.Current.GoToAsync("..");

    partial void OnSelectedSteeringOptionChanged(SteeringOption value) => ApplyFilterOption();
    partial void OnStringInNameFilterChanged(string value) => ApplyFilterOption();
    partial void OnMinWeightFilterChanged(int value) => ApplyFilterOption();
}