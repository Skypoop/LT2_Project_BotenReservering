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

        Tabs = new ObservableCollection<TabItem>();
        if(roles.Any(r => r.RoleName == "Lid"))
            Tabs.Add(new TabItem("boat_image.png", typeof(BoatTypesView)));
        
        if(roles.Any(r => r.RoleName == "WedstrijdCommissaris"))
            Tabs.Add(new TabItem("competition_image.png", typeof(CompetitionView)));
        

        SelectedTab = Tabs[0];

        SelectTabCommand = new Command<TabItem>(tab =>
        {
            SelectedTab = tab;
        });
    }
}