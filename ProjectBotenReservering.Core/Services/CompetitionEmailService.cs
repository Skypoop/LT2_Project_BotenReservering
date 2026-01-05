using ProjectBotenReservering.Core.Interfaces.Services;

namespace ProjectBotenReservering.Core.Services;

public class CompetitionEmailService : ICompetitionEmailService
{
    private readonly ISmtpMailService _smtpMailService;

    public CompetitionEmailService(ISmtpMailService smtpMailService)
    {
        _smtpMailService = smtpMailService;
    }

    public async Task SendPreparedEmailsAsync(List<(string Email, string Subject, string Body)> emails)
    {
        List<Task> emailTasks = new List<Task>();

        foreach ((string Email, string Subject, string Body) emailData in emails)
        {
            List<string> receivers = new List<string> { emailData.Email };
            emailTasks.Add(_smtpMailService.SendMailAsync(receivers, emailData.Subject, emailData.Body));
        }

        await Task.WhenAll(emailTasks);
    }
}