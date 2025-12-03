using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces.Context;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels
{
    public partial class LoginViewModel(IAuthService authService, IClientContext clientContext) : BaseViewModel
    {
        [ObservableProperty]
        private string email = "";

        [ObservableProperty]
        private string password = "";

        [ObservableProperty]
        private string loginMessage;

        [ObservableProperty]
        private bool isPasswordHidden = true;

        [RelayCommand]
        private void Login()
        {
            Client? authenticatedClient = authService.Login(Email, Password);

            if (authenticatedClient != null)
            {
                LoginMessage = $"Welkom {authenticatedClient.FullName}!";
                clientContext.SetCurrentClientId(authenticatedClient.Id);
                Application.Current.MainPage = new AppShell();
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
            //logic for navigating to registering page goes here.
        }
    }
}