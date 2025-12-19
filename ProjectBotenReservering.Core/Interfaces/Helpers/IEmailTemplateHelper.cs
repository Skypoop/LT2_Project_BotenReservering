using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Helpers;

public interface IEmailTemplateHelper
{
    Task<(string Subject, string Body)> RenderCompetitionConfirmationAsync(CompetitionEmailContext context, Client currentClient, int boatId);
}