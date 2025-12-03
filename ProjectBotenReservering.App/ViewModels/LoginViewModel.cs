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
        private string _email = "";

        [ObservableProperty]
        private string _password = "";

        [ObservableProperty]
        private string _loginMessage;

        [ObservableProperty]
        private bool _isPasswordHidden = true;

        [RelayCommand]
        private void Login()
        {
            Client? authenticatedClient = authService.Login(Email, Password);

            if (authenticatedClient != null)
            {
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