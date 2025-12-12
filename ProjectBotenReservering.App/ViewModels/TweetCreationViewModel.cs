using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class TweetCreationViewModel : BaseViewModel
{
    [ObservableProperty] 
    public partial string PageWelcomeMessage { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial string TweetContent { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial string SelectedFileName { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial ImageSource? SelectedImagePreview { get; set; }

    [ObservableProperty] 
    public partial bool IsImagePreviewVisible { get; set; }

    [ObservableProperty]
    public partial bool IsTweetContentEditableByUser { get; set; }
    
    private readonly IClientService _clientService;
    
    public TweetCreationViewModel(IClientService clientService)
    {
        _clientService = clientService;

        SetupPage();
        // Call a tweet generation service here to generate Tweet content
    }

    private async void SetupPage()
    {
        Client? currentClient = _clientService.GetCurrentClient();
        if (currentClient == null)
        {
            await Shell.Current.DisplayAlert("Client not found", "De huidige client is onbekend, neem contact op met een beheerder.", "OK");
            await Shell.Current.GoToAsync(nameof(LoginView));
        }
        else
            SetupPageWelcomeMessage(currentClient.FullName);
    }
    
    private void SetupPageWelcomeMessage(string username)
    {
        PageWelcomeMessage = $"Hallo {username} ik maak voor jouw een tweet concept! " +
                             "Pas gerust aan wat je wilt zodra hij gegenereerd is! " +
                             "Alle wijzigingen worden automatisch opgeslagen.";
    }

    [RelayCommand]
    private async Task PickFileAsync()
    {
        FileResult? result = await FilePicker.Default.PickAsync(PickOptions.Images);
        if (result != null)
        {
            SelectedFileName = result.FileName;

            Stream stream = await result.OpenReadAsync();
            SelectedImagePreview = ImageSource.FromStream(() => stream);
            IsImagePreviewVisible = SelectedImagePreview != null;
        }
    }
    
    [RelayCommand]
    private void PublishTweet()
    {
        // Add implementation to publish tweet through service
    }
}