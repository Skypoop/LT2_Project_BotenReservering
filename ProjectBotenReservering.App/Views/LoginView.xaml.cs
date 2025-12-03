using ProjectBotenReservering.App.ViewModels;

namespace ProjectBotenReservering.App.Views;

public partial class LoginView : ContentPage
{
    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}