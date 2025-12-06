using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ProjectBotenReservering.App.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _sweepLevel = string.Empty;

    [ObservableProperty]
    private string _scullLevel = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _roles;

    [ObservableProperty]
    private string _selectedRole;

    [ObservableProperty]
    private bool _isNameInvalid;

    [ObservableProperty]
    private bool _isEmailInvalid;

    [ObservableProperty]
    private bool _isSweepLevelInvalid;

    [ObservableProperty]
    private bool _isScullLevelInvalid;

    [ObservableProperty]
    private bool _isPasswordMismatch;

    [ObservableProperty]
    private bool _isPasswordHidden = true;

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;

        _roles = new ObservableCollection<string>
        {
            "Lid",
            "Nieuw Lid",
            "Gast"
        };
        _selectedRole = _roles[0];
    }

    [RelayCommand]
    private void TogglePassword()
    {
        IsPasswordHidden = !IsPasswordHidden;
    }

    [RelayCommand]
    private void ValidateName()
    {
        string trimmedName = Name?.Trim() ?? string.Empty;
        string[] parts = trimmedName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Regex nameRegex = new(@"^[\p{L}\s\-\']+$");

        IsNameInvalid = string.IsNullOrWhiteSpace(trimmedName) || parts.Length < 2 || !nameRegex.IsMatch(trimmedName);
    }

    [RelayCommand]
    private void ValidateEmail()
    {
        string trimmedEmail = Email?.Trim() ?? string.Empty;
        Regex regex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        IsEmailInvalid = string.IsNullOrWhiteSpace(trimmedEmail) || !regex.IsMatch(trimmedEmail);
    }

    [RelayCommand]
    private void ValidateSweepLevel()
    {
        if (string.IsNullOrWhiteSpace(SweepLevel))
        {
            IsSweepLevelInvalid = false;
            return;
        }
        bool isInt = int.TryParse(SweepLevel, out int val);
        IsSweepLevelInvalid = !isInt || val < 0 || val > 3;
    }

    [RelayCommand]
    private void ValidateScullLevel()
    {
        if (string.IsNullOrWhiteSpace(ScullLevel))
        {
            IsScullLevelInvalid = false;
            return;
        }
        bool isInt = int.TryParse(ScullLevel, out int val);
        IsScullLevelInvalid = !isInt || val < 0 || val > 3;
    }

    [RelayCommand]
    private void ValidatePasswordMatch()
    {
        IsPasswordMismatch =
        !string.IsNullOrEmpty(Password) &&
        !string.IsNullOrEmpty(ConfirmPassword) &&
        Password != ConfirmPassword;
    }


    [RelayCommand]
    private async Task Register()
    {
        ValidateName();
        ValidateEmail();
        ValidateSweepLevel();
        ValidateScullLevel();
        ValidatePasswordMatch();

        if (IsNameInvalid || IsEmailInvalid || IsSweepLevelInvalid || IsScullLevelInvalid || IsPasswordMismatch)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Fout", "Vul alle verplichte velden in.", "OK");
            return;
        }

        if (_authService.EmailExists(Email))
        {
            IsEmailInvalid = true;
            await Shell.Current.DisplayAlert("Fout", "Dit emailadres is al in gebruik.", "OK");
            return;
        }

        _ = int.TryParse(SweepLevel, out int sweepLevelInt);
        _ = int.TryParse(ScullLevel, out int scullLevelInt);

        string? clubValue;
        clubValue = (SelectedRole == "Gast") ? "Extern" : "Remus Invictus";

        Client newClient = new Client(
                Name,
                Email,
                scullLevelInt,
                sweepLevelInt,
                clubValue,
                true,    // Approved by default (for now)
                string.Empty,
                0
            );

        bool success = _authService.Register(newClient, Password, SelectedRole);

        if (success)
        {
            await Shell.Current.DisplayAlert("Succes", "Account succesvol aangemaakt.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await Shell.Current.DisplayAlert("Fout", "Er is iets misgegaan tijdens de registratie.", "OK");
        }
    }

    [RelayCommand]
    private async Task NavigateToLogin()
    {
        await Shell.Current.GoToAsync("..");
    }
}