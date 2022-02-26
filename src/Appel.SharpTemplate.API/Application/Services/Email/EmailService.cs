using Appel.SharpTemplate.API.Application.Interfaces;
using Appel.SharpTemplate.Infrastructure.Application;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Application.Services;

public class EmailService : IEmailService
{
    private readonly IOptions<AppSettings> _appSettings;

    public EmailService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public async Task SendAsync(string subject, string message, string email)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress("Appel Sharp Template", _appSettings.Value.EmailUser));
        mimeMessage.To.Add(new MailboxAddress(string.Empty, email));
        mimeMessage.Subject = subject;
        mimeMessage.Body = new TextPart("html") { Text = message };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_appSettings.Value.SmtpHost, int.Parse(_appSettings.Value.SmtpPort));
            await client.AuthenticateAsync(_appSettings.Value.EmailUser, _appSettings.Value.EmailPassword);
            await client.SendAsync(mimeMessage);
            await client.DisconnectAsync(true);
        }
    }
}
