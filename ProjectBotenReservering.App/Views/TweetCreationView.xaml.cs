using ProjectBotenReservering.App.ViewModels;

namespace ProjectBotenReservering.App.Views;

public partial class TweetCreationView : ContentPage
{
    public TweetCreationView(TweetCreationViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}