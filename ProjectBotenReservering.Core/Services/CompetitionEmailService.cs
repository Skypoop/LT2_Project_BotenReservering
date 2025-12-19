using System.Collections.ObjectModel;
using ProjectBotenReservering.Core.Interfaces.Helpers;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class CompetitionEmailService : ICompetitionEmailService
{
    private readonly ISmtpMailService _smtpMailService;
    private readonly IEmailTemplateHelper _emailTemplateRenderer;

    public CompetitionEmailService(ISmtpMailService smtpMailService, IEmailTemplateHelper emailTemplateRenderer)
    {
        _smtpMailService = smtpMailService;
        _emailTemplateRenderer = emailTemplateRenderer;
    }

    public async Task SendCompetitionConfirmationEmailsAsync(CompetitionEmailContext context)
    {
        foreach (KeyValuePair<int, ObservableCollection<Client>> entry in context.ClientsByBoatId)
        {
            await ProcessTeamEmailsAsync(entry.Key, entry.Value, context);
        }
    }

    private async Task ProcessTeamEmailsAsync(int boatId, ObservableCollection<Client> teamMembers, CompetitionEmailContext context)
    {
        if (teamMembers.Count == 0) return;

        foreach (Client client in teamMembers)
        {
            if (string.IsNullOrEmpty(client.Email)) continue;

            (string subject, string body) = await _emailTemplateRenderer.RenderCompetitionConfirmationAsync(context, client, boatId);

            if (!string.IsNullOrEmpty(body))
            {
                await SendEmailToClientAsync(client.Email, subject, body);
            }
        }
    }

    private async Task SendEmailToClientAsync(string emailAddress, string subject, string body)
    {
        List<string> receivers = new List<string> { emailAddress };
        await _smtpMailService.SendMailAsync(receivers, subject, body);
    }
}