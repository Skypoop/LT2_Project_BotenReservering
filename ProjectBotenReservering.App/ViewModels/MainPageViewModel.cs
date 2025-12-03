using System.Collections.ObjectModel;
using System.Windows.Input;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Mappers;

namespace ProjectBotenReservering.App.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    public ObservableCollection<TabItem> Tabs { get; }

    private TabItem _selectedTab;
    public TabItem SelectedTab
    {
        get => _selectedTab;
        set => SetProperty(ref _selectedTab, value);
    }

    public ICommand SelectTabCommand { get; }

    public MainPageViewModel(BoatTypesViewModel boatTypesViewModel)
    {
        Tabs = new ObservableCollection<TabItem>
        {
            new TabItem("calendar_regular_full", new BoatTypesView(boatTypesViewModel))
        };

        SelectedTab = Tabs[0];

        SelectTabCommand = new Command<TabItem>(tab =>
        {
            SelectedTab = tab;
        });
    }
}
