using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface ICompetitionEmailService
{
    Task SendPreparedEmailsAsync(List<(string Email, string Subject, string Body)> emails);
}