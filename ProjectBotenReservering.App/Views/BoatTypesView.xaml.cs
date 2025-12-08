using ProjectBotenReservering.App.ViewModels;
using ProjectBotenReservering.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBotenReservering.App.Views;

public partial class BoatTypesView : ContentView
{
    public BoatTypesView(BoatTypesViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
        
        Loaded += BoatTypesView_Loaded;
    }
    
    private async void BoatTypesView_Loaded(object? sender, EventArgs e)
    {
        if (BindingContext is BoatTypesViewModel vm)
        {
            await vm.OnApearing();
        }
    }
    private void OnBoatTypeTapped(object sender, TappedEventArgs e)
    {
        if (sender is not BindableObject bindable) return;
        if (bindable.BindingContext is not BoatTypeUiItem tappedItem) return;

        if (this.BindingContext is BoatTypesViewModel viewModel)
        {
            if (viewModel.SelectBoatTypeCommand.CanExecute(tappedItem))
            {
                viewModel.SelectBoatTypeCommand.Execute(tappedItem);
            }
        }
    }
}