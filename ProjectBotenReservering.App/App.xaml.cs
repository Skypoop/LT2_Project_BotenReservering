using ProjectBotenReservering.App.Views;

namespace ProjectBotenReservering.App
{
    public partial class App : Application
    {
        public App(LoginView loginView)
        {
            InitializeComponent();
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}