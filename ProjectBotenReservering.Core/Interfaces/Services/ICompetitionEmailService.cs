using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface ICompetitionEmailService
{
    Task SendCompetitionConfirmationEmailsAsync(CompetitionEmailContext context);
}