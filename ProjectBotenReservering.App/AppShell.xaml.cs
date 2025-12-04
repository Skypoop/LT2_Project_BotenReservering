namespace ProjectBotenReservering.App;

using ProjectBotenReservering.App.Views;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
        Routing.RegisterRoute(nameof(HomePageView), typeof(HomePageView));
        Routing.RegisterRoute(nameof(BoatTypesView), typeof(BoatTypesView));
        Routing.RegisterRoute(nameof(ReservationFormView), typeof(ReservationFormView));
    }
}