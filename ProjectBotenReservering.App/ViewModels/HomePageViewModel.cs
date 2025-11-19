using ProjectBotenReservering.Core.Interfaces.Services;

namespace ProjectBotenReservering.App.ViewModels;

public class HomePageViewModel : BaseViewModel
{
    public HomePageViewModel(IMailService mailService)
    {
        IMailService _mailService = mailService;
    }
}