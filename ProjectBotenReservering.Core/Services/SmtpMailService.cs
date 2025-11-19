using ProjectBotenReservering.Core.Helpers;
using ProjectBotenReservering.Core.Interfaces.Services;
using System.Diagnostics;
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
            validate(value);

            _server = value;
        }
    }

    public string? port
    {
        get { return _port; }
        set
        {
            validate(value);

            _port = value;
        }
    }

    public string? username
    {
        get { return _username; }
        set
        {
            validate(value);

            _username = value;
        }
    }

    public string? password
    {
        get { return _password; }
        set
        {
            validate(value);

            _password = value;
        }
    }

    public SmtpMailService()
    {
        server = MailConnectionHelper.mailConnectionStringValue("server");
        port = "587";
        username = "keyshawn42@ethereal.email";
        password = "MHwe9Cr1zH23MDZw2Q";
    }

    private void validate(string? value)
    {
        try
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Can not find the SMTP args");
            }
        } catch(ArgumentException argsException)
        {
            Debug.WriteLine(argsException.Message);
        }
    }
    public async Task sendMailAsync(List<string> receivers, string subject, string body)
    {
        try
        {
            var smtp = new SmtpClient(server) { Port = Int32.Parse(port), EnableSsl = true, Credentials = new NetworkCredential(username, password) };
            var message = new MailMessage()
            {
                From = new MailAddress(username),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            if(receivers.Any(reciver => !isEmailValid(reciver) || string.IsNullOrEmpty(reciver)))
            {
                throw new ArgumentException("Reciver is no valid email adress");
            }

            receivers.ForEach(reciver => message.To.Add(reciver));

            await smtp.SendMailAsync(message);
        } catch(ArgumentException argsException)
        {
            Debug.WriteLine(argsException.Message);
        } catch(Exception exception)
        {
            Debug.WriteLine($"{exception.Message}");
        }
    }

    public bool isEmailValid(string? email)
    {
        if(email.Contains("@"))
        {
            return true;
        }

        return false;
    }
}