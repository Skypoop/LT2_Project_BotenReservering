using ProjectBotenReservering.App.ViewModels;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.Views;

public partial class BoatTypeSelectionCompetitionView : ContentPage
{
    public BoatTypeSelectionCompetitionView(BoatTypeSelectionCompetitionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Loaded += BoatTypeSelectionMatchView_Loaded;
    }

    private async void BoatTypeSelectionMatchView_Loaded(object? sender, EventArgs e)
    {
        if (BindingContext is BoatTypeSelectionCompetitionViewModel vm)
        {
            await vm.OnAppearing();
        }
    }

    private void OnBoatTypeTapped(object sender, TappedEventArgs e)
    {
        if (sender is not BindableObject bindable) return;
        if (bindable.BindingContext is not BoatTypeUiItem tappedItem) return;

        if (this.BindingContext is BoatTypeSelectionCompetitionViewModel viewModel)
        {
            if (viewModel.SelectBoatTypeCommand.CanExecute(tappedItem))
            {
                viewModel.SelectBoatTypeCommand.Execute(tappedItem);
            }
        }
    }
}