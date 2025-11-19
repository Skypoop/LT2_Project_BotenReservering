using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectBotenReservering.App.ViewModels;

namespace ProjectBotenReservering.App.Views;

public partial class BootTypesView : ContentPage
{
    public BootTypesView(BootTypesViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}