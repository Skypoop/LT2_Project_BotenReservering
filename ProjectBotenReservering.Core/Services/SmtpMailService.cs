using System.Net;
using System.Net.Mail;
using ProjectBotenReservering.Core.Interfaces.Services;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Services;

public class SmtpMailService : ISmtpMailService
{
    private readonly MailSettings _settings;

    public SmtpMailService(MailSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task SendMailAsync(List<string> receivers, string subject, string body)
    {
        using SmtpClient smtp = new SmtpClient(_settings.Server);
        smtp.Port = _settings.Port;
        smtp.EnableSsl = true;
        smtp.Credentials = new NetworkCredential(_settings.Username, _settings.Password);

        foreach (string receiver in receivers)
        {
            using MailMessage message = new MailMessage();
            message.From = new MailAddress(_settings.Username);
            message.To.Add(receiver);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            await smtp.SendMailAsync(message);
        }
    }
}