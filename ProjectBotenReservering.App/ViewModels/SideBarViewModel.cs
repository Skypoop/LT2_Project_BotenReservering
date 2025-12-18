using System.Collections.ObjectModel;
using System.Windows.Input;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
namespace ProjectBotenReservering.App.ViewModels;

public partial class SideBarViewModel : BaseViewModel
{
    private readonly IClientService _clientService;
    private readonly IAuthService _authService;
    
    public ObservableCollection<TabItem> Tabs { get; }

    private TabItem? _selectedTab;
    public TabItem? SelectedTab
    {
        get => _selectedTab;
        set => SetProperty(ref _selectedTab, value);
    }

    public ICommand SelectTabCommand { get; }

    public SideBarViewModel(IClientService clientService, IAuthService authService)
    {
        _clientService = clientService;
        _authService = authService;
        
        Client? curClient = _clientService.GetCurrentClient();
        if (curClient == null)
            throw new Exception("No client found");
        
        ClientRole[] roles = _authService.GetClientRoles(curClient.Id);

        TabItem[] tabItems =       
        {
            new ("boat_image.png", typeof(BoatTypesView)),
            new ("competition_image.png", typeof(CompetitionView)),
        };
        
        Tabs = new ObservableCollection<TabItem>(_authService.GetAuthorisedTabs(curClient.Id, tabItems));

        SelectedTab = Tabs[0];

        SelectTabCommand = new Command<TabItem>(tab =>
        {
            SelectedTab = tab;
        });
    }
}