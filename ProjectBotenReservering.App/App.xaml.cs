namespace ProjectBotenReservering.App;

public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; private set; }
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        ServiceProvider = serviceProvider;
    }
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}