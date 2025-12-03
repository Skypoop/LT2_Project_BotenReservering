using ProjectBotenReservering.App.Views;

namespace ProjectBotenReservering.App;

public partial class App : Application
{
    public App(LoginView loginView)
    {
        InitializeComponent();

        MainPage = loginView;
    }
}