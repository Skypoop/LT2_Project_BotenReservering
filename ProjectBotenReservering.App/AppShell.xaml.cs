namespace ProjectBotenReservering.App;

using ProjectBotenReservering.App.Views;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
        Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
        Routing.RegisterRoute(nameof(ReservationFormView), typeof(ReservationFormView));
        Routing.RegisterRoute(nameof(TweetCreationView), typeof(TweetCreationView));
    }
}