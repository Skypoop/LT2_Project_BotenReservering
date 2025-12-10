using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

public partial class TweetCreationViewModel : BaseViewModel
{
    [ObservableProperty] public partial string PageWelcomeMessage { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial string TweetContent { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial string SelectedFileName { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial ImageSource? SelectedImagePreview { get; set; }
    public bool IsImagePreviewVisible => SelectedImagePreview != null;

    public bool TweetContentEditableByUser { get; set; } = false;
    
    private readonly IClientService _clientService;
    
    public TweetCreationViewModel(IClientService clientService)
    {
        _clientService = clientService;

        SetupPage();
        // Call a tweet generation service here to generate Tweet content
    }

    private void SetupPage()
    {
        Client? currentClient = _clientService.GetCurrentClient();
        if (currentClient == null)
            Console.WriteLine($"Called before login of client");
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
            OnPropertyChanged(nameof(IsImagePreviewVisible));
        }
    }
    
    [RelayCommand]
    private void PublishTweet()
    {
        // Add implementation to publish tweet through service
    }
}