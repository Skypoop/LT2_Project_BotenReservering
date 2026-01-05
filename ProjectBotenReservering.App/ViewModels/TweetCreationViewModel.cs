using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectBotenReservering.App.Views;
using ProjectBotenReservering.Core.Data.Helpers;
using ProjectBotenReservering.Core.Exceptions;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.ViewModels;

[QueryProperty(nameof(CompetitionContext), "context")]
public partial class TweetCreationViewModel : BaseViewModel
{
    private readonly IClientService _clientService;
    private readonly ITweetService _tweetService;

    [ObservableProperty]
    public partial string PageWelcomeMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string TweetContent { get; set; } = string.Empty;
    [ObservableProperty]
    public partial int ContentLength { get; set; }

    [ObservableProperty]
    public partial string CompetitionContext { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SelectedFileName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ImageSource? SelectedImagePreview { get; set; }

    [ObservableProperty]
    public partial bool IsImagePreviewVisible { get; set; }

    [ObservableProperty]
    public partial bool IsTweetContentEditableByUser { get; set; }

    private byte[]? _selectedImageBytes;
    public TweetCreationViewModel(IClientService clientService, ITweetService tweetService)
    {
        _clientService = clientService;
        _tweetService = tweetService;
    }

    partial void OnCompetitionContextChanged(string value)
    {
        SetupPage();
    }

    partial void OnTweetContentChanged(string value)
    {
        ContentLength = TweetContent.Length;
    }

    private async Task GenerateTweet()
    {
        try
        {
            string response = await _tweetService.GenerateCompetitionTweetAsync(CompetitionContext);

            TweetContent = response;
            IsTweetContentEditableByUser = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            TweetContent = "Kon geen tweet genereren. Probeer het later opnieuw.";
        }
    }

    private async void SetupPage()
    {
        try
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
                // Only generate if we haven't already
                if (string.IsNullOrWhiteSpace(TweetContent))
                {
                    await GenerateTweet();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SetupPage: {ex.Message}");
        }
    }

    private void SetupPageWelcomeMessage(string username)
    {
        PageWelcomeMessage = $"Hallo {username}, er wordt een promotioneel tweet concept gegenereerd. " +
                             "Pas gerust aan wat je wilt zodra hij gegenereerd is! " +
                             "Alle wijzigingen worden automatisch opgeslagen.";
    }

    [RelayCommand]
    private async Task PickFileAsync()
    {
        FileResult? result = await FilePicker.Default.PickAsync(PickOptions.Images);
        if (result == null) return;

        Stream stream = await result.OpenReadAsync();
        _selectedImageBytes = await StreamHelper.ReadStreamToBytesAsync(stream);
        stream.Dispose();

        SelectedImagePreview = ImageSource.FromStream(() => new MemoryStream(_selectedImageBytes));

        SelectedFileName = result.FileName;
        IsImagePreviewVisible = true;
    }


    [RelayCommand]
    private async Task PublishTweet()
    {
        string responseMessage;

        try
        {
            if (_selectedImageBytes != null && SelectedFileName != null)
            {
                responseMessage = await _tweetService.PublishTweetWithMediaAsync(TweetContent, _selectedImageBytes, SelectedFileName);
            }
            else
            {
                responseMessage = await _tweetService.PublishTweetAsync(TweetContent);
            }
        }
        catch (TweetPostException ex)
        {
            responseMessage = $"Fout: {ex.Message}";
        }

        await ShowFeedbackAndNavigate(responseMessage);
    }

    private async Task ShowFeedbackAndNavigate(string message)
    {
        await Shell.Current.DisplayAlert("Info", message, "OK");
        await Shell.Current.GoToAsync("..");
    }
}