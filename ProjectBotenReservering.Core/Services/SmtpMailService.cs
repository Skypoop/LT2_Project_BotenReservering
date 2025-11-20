using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Services;
using System.Net;
using System.Net.Mail;

namespace ProjectBotenReservering.Core.Services;

public class SmtpMailService : IMailService
{
    private string? _server;
    private string? _port;
    private string? _username;
    private string? _password;

    public string? server
    {
        get { return _server; }
        set
        {
            Validate(value, nameof(server));

            _server = value;
        }
    }

    public string? port
    {
        get { return _port; }
        set
        {
            Validate(value, nameof(port));

            _port = value;
        }
    }

    public string? username
    {
        get { return _username; }
        set
        {
            Validate(value, nameof(username));

            _username = value;
        }
    }

    public string? password
    {
        get { return _password; }
        set
        {
            Validate(value, nameof(password));

            _password = value;
        }
    }

    public SmtpMailService()
    {
        server = MailConnectionHelper.MailConnectionStringValue("server");
        port = MailConnectionHelper.MailConnectionStringValue("port");
        username = MailConnectionHelper.MailConnectionStringValue("username");
        password = MailConnectionHelper.MailConnectionStringValue("password");
    }

    private void Validate(string? value, string parameter)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new Exception($"SMTP parameter '{parameter}' is missing or empty in appsettings.json");
        }
    }

    public async Task SendMailAsync(List<string> receivers, string subject, string body)
    {
        var smtp = new SmtpClient(server) { Port = Int32.Parse(port), EnableSsl = true, Credentials = new NetworkCredential(username, password) };
        var message = new MailMessage()
        {
            From = new MailAddress(username),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        if(receivers.Any(reciver => !IsEmailValid(reciver) || string.IsNullOrEmpty(reciver)))
        {
            throw new ArgumentException("Reciver is no valid email adress, must contain @");
        }

        receivers.ForEach(reciver => message.To.Add(reciver));

        await smtp.SendMailAsync(message);
    }

    public bool IsEmailValid(string? email)
    {
        if(email.Contains("@"))
        {
            return true;
        }

        return false;
    }
}