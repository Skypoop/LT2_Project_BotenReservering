using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectBotenReservering.App.ViewModels;

namespace ProjectBotenReservering.App.Views;

public partial class ReservationFormView : ContentPage
{
    public ReservationFormView(ReservationFormViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}