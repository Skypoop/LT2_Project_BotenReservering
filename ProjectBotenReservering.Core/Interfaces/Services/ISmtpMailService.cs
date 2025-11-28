namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface ISmtpMailService
{
    public Task SendMailAsync(List<string> receivers, string subject, string body);
}