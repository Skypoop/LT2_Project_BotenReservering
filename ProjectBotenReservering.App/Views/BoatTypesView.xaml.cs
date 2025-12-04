using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectBotenReservering.App.ViewModels;

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
}