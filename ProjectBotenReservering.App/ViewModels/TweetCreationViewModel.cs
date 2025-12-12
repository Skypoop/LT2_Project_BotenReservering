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
    private readonly ILlmService _llmService;

    public TweetCreationViewModel(IClientService clientService, ILlmService llmService)
    {
        _clientService = clientService;
        _llmService = llmService;

        SetupPage();
    }

    private async Task GenerateTweet()
    {
        try
        {
            string response = await _llmService.GenerateTextWithContextAsync("Use the following context: Context not available. Make up the location, date, time, teams, etc.", "You are a dutch promotional tweet generator for a rowing club called Remus Invictus. You create engaging, promotional tweets about rowing competitions in dutch, in 280 characters. You always only output the tweet text without any additional commentary. You use the context provided (name, teams, optionally location, date, time, etc). Add hashtags like #RemusInvictus and #Roeien where deemed necessary. Use a few emojis.");
            TweetContent = response;
            IsTweetContentEditableByUser = true;
            Console.WriteLine(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating tweet: {ex.Message}");
            TweetContent = "Kon geen tweet genereren. Probeer het later opnieuw.";
        }
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
        {
            SetupPageWelcomeMessage(currentClient.FullName);
            await GenerateTweet();
        }
    }

    private void SetupPageWelcomeMessage(string username)
    {
        PageWelcomeMessage = $"Hallo {username} ik maak voor jou een tweet concept! " +
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