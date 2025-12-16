using ProjectBotenReservering.App.ViewModels;

namespace ProjectBotenReservering.App.Views;

public partial class CompetitionView : ContentView
{
    private readonly CompetitionViewModel _viewModel;

    public CompetitionView(CompetitionViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
        _viewModel = viewModel;
        Loaded += OnLoaded;
    }

    void OnLoaded(object? sender, EventArgs e)
    {
        _viewModel.FillBoatCompetitionsList();
    }
}