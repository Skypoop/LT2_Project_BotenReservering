using ProjectBotenReservering.App.ViewModels;
using ProjectBotenReservering.Core.Models;

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

    private void OnShowWarningClicked(object sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: Client client } &&
            this.BindingContext is CompetitionViewModel vm)
        {
            if (vm.ShowQualificationWarningCommand.CanExecute(client))
            {
                vm.ShowQualificationWarningCommand.Execute(client);
            }
        }
    }

    private void OnRemoveClientClicked(object sender, EventArgs e)
    {
        if (sender is BindableObject { BindingContext: Client client } &&
            this.BindingContext is CompetitionViewModel vm)
        {
            if (vm.RemoveClientCommand.CanExecute(client))
            {
                vm.RemoveClientCommand.Execute(client);
            }
        }
    }
}