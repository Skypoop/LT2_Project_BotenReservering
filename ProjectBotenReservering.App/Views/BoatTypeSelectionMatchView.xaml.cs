using ProjectBotenReservering.App.ViewModels;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.App.Views;

public partial class BoatTypeSelectionMatchView : ContentPage
{
    public BoatTypeSelectionMatchView(BoatTypeSelectionMatchViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Loaded += BoatTypeSelectionMatchView_Loaded;
    }

    private async void BoatTypeSelectionMatchView_Loaded(object? sender, EventArgs e)
    {
        if (BindingContext is BoatTypeSelectionMatchViewModel vm)
        {
            await vm.OnAppearing();
        }
    }

    private void OnBoatTypeTapped(object sender, TappedEventArgs e)
    {
        if (sender is not BindableObject bindable) return;
        if (bindable.BindingContext is not BoatTypeUiItem tappedItem) return;

        if (this.BindingContext is BoatTypeSelectionMatchViewModel viewModel)
        {
            if (viewModel.SelectBoatTypeCommand.CanExecute(tappedItem))
            {
                viewModel.SelectBoatTypeCommand.Execute(tappedItem);
            }
        }
    }
}