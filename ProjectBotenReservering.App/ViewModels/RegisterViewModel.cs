using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;
using System.Collections.ObjectModel;

namespace ProjectBotenReservering.App.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string Email { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string SweepLevel { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string ScullLevel { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string ConfirmPassword { get; set; } = string.Empty;
    [ObservableProperty]
    public partial ObservableCollection<string> Roles { get; set; } = new ObservableCollection<string>();
    [ObservableProperty]
    public partial string SelectedRole { get; set; } = string.Empty;
    [ObservableProperty]
    public partial bool IsNameInvalid { get; set; }
    [ObservableProperty]
    public partial bool IsEmailInvalid { get; set; }
    [ObservableProperty]
    public partial bool IsSweepLevelInvalid { get; set; }
    [ObservableProperty]
    public partial bool IsScullLevelInvalid { get; set; }
    [ObservableProperty]
    public partial bool IsPasswordMismatch { get; set; }
    [ObservableProperty]
    public partial bool IsPasswordHidden { get; set; } = true;

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;

        Roles = new ObservableCollection<string>
        {
            "Lid",
            "Nieuw Lid",
            "Gast"
        };
        SelectedRole = Roles[0];
    }

    [RelayCommand]
    private void TogglePassword()
    {
        IsPasswordHidden = !IsPasswordHidden;
    }

    [RelayCommand]
    private void ValidateName()
    {
        IsNameInvalid = !ValidationHelper.IsValidName(Name);
    }

    [RelayCommand]
    private void ValidateEmail()
    {
        IsEmailInvalid = !ValidationHelper.IsValidEmail(Email);
    }

    [RelayCommand]
    private void ValidateSweepLevel()
    {
        IsSweepLevelInvalid = !ValidationHelper.IsValidLevel(SweepLevel);
    }

    [RelayCommand]
    private void ValidateScullLevel()
    {
        IsScullLevelInvalid = !ValidationHelper.IsValidLevel(ScullLevel);
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
                true,
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