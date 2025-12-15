using ProjectBotenReservering.App.ViewModels;

namespace ProjectBotenReservering.App.Views;

public partial class CompetitionView : ContentView
{
    public CompetitionView(CompetitionViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}