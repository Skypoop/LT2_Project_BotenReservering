using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces.Context;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using ProjectBotenReservering.App.Views;

namespace ProjectBotenReservering.App.ViewModels
{
    public partial class LoginViewModel(IAuthService authService, IClientContext clientContext) : BaseViewModel
    {
        [ObservableProperty]
        private string _email = "";

        [ObservableProperty]
        private string _password = "";

        [ObservableProperty]
        private string? _loginMessage;

        [ObservableProperty]
        private bool _isPasswordHidden = true;

        [RelayCommand]
        private async Task Login()
        {
            Client? authenticatedClient = authService.Login(Email, Password);

            if (authenticatedClient != null)
            {
                string role = authService.GetUserRole(authenticatedClient.Id);
                    
                if(string.Equals(role, "Gast", StringComparison.OrdinalIgnoreCase))
                {
                    await Shell.Current.DisplayAlert("Toegang Geweigerd", "Er is nog geen functioneel scherm beschikbaar voor een account ingelogd als Gast.", "OK");
                    return;
                }

                clientContext.SetCurrentClientId(authenticatedClient.Id);
                await Shell.Current.GoToAsync(nameof(BoatTypesView));
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