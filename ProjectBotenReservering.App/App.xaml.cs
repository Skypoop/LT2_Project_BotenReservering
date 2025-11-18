using ProjectBotenReservering.App.ViewModels;
using ProjectBotenReservering.App.Views;

namespace ProjectBotenReservering.App
{
    public partial class App : Application
    {
        public App(HomePageViewModel viewModel)
        {
            InitializeComponent();

            //MainPage = new AppShell();
            MainPage = new HomePageView(viewModel);
        }
    }
}