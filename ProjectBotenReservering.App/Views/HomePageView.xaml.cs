using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectBotenReservering.App.ViewModels;

namespace ProjectBotenReservering.App.Views;

public partial class HomePageView : ContentPage
{
    public HomePageView(HomePageViewModel viewModel)
    {
        InitializeComponent();
        
        BindingContext = viewModel;
    }
}