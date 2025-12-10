using ProjectBotenReservering.App.ViewModels;

namespace ProjectBotenReservering.App.Views;

public partial class TweetCreationView : ContentView
{
    public TweetCreationView(TweetCreationViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
}