using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using ProjectBotenReservering.Core.Interfaces.Context;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _email = "";

        [ObservableProperty]
        private string _password = "";

        [ObservableProperty]
        private string _loginMessage;

        [ObservableProperty]
        private bool _isPasswordHidden = true;

        private readonly IAuthService _authService;
        private readonly IClientContext _clientContext;
        private readonly IClientRepository _clientRepository;
        private readonly MailSettings _mailSettings;
        
        public LoginViewModel(IAuthService authService, IClientContext clientContext, MailSettings mailSettings, IClientRepository clientRepository)
        {
            _authService = authService;
            _clientContext = clientContext;
            _mailSettings = mailSettings;
            _clientRepository = clientRepository;
        }
        
        [RelayCommand]
        private void Login()
        {
            Client? authenticatedClient = _authService.Login(Email, Password);

            if (authenticatedClient != null)
            {
                _clientContext.SetCurrentClientId(authenticatedClient.Id);
                SetNewMailSettings(authenticatedClient);
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                LoginMessage = "Ongeldige inloggegevens.";
            }
        }

        private void SetNewMailSettings(Client newClient)
        {
            _mailSettings.Username = newClient.Email;
            _mailSettings.Password = newClient.PasswordHash;
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