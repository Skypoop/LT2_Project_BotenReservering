using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Interfaces.Context;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        public partial string Email { get; set; } = "";
        [ObservableProperty]
        public partial string Password { get; set; } = "";
        [ObservableProperty]
        public partial string? LoginMessage { get; set; }
        [ObservableProperty]
        public partial bool IsPasswordHidden { get; set; } = true;

        private readonly IAuthService _authService;
        private readonly IClientContext _clientContext;
        private readonly IClientRepository _clientRepository;
        private readonly MailSettings _mailSettings;
        private readonly IClientService _clientService;

        public LoginViewModel(IAuthService authService, IClientContext clientContext, MailSettings mailSettings, IClientRepository clientRepository, IClientService clientService)
        {
            _authService = authService;
            _clientContext = clientContext;
            _mailSettings = mailSettings;
            _clientRepository = clientRepository;
            _clientService = clientService;
        }

        [RelayCommand]
        private async Task Login()
        {
            Client? authenticatedClient = _authService.Login(Email, Password);

            if (authenticatedClient != null)
            {
                ClientRole[] roles = _authService.GetClientRoles(authenticatedClient.Id);

                if (roles.Any(r => r.RoleName == "Gast" || r.RoleName == "Nieuw Lid"))
                {
                    await Shell.Current.DisplayAlert("Toegang Geweigerd", $"Er is nog geen functioneel scherm beschikbaar voor een account ingelogd als Gast of Niew Lid", "OK");
                    return;
                }

                _clientContext.SetCurrentClientId(authenticatedClient.Id);
                await Shell.Current.GoToAsync($"//{nameof(BoatTypesView)}");
            }

            else
            {
                LoginMessage = "Ongeldige inloggegevens.";
            }
        }

        [RelayCommand]
        private void TogglePassword()
        {
            IsPasswordHidden = !IsPasswordHidden;
        }

        [RelayCommand]
        private static async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync(nameof(RegisterView));
        }
    }
}