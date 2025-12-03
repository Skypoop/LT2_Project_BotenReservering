using ProjectBotenReservering.App.ViewModels;
using ProjectBotenReservering.App.Views;

namespace ProjectBotenReservering.App;

public partial class App : Application
{
    public App(AppShell appShell)
    {
        InitializeComponent();

        MainPage = appShell;
    }
}