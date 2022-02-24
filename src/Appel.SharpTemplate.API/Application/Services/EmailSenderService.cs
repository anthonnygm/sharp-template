using Appel.SharpTemplate.API.Application.DTOs.Email;
using Appel.SharpTemplate.API.Extensions;
using Appel.SharpTemplate.Infrastructure.Application;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Application.Services;

public class EmailSenderService
{
    private readonly IOptions<AppSettings> _appSettings;

    public EmailSenderService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public async Task SendForgotPasswordEmailAsync(int userId, string email)
    {
        var message = await LoadEmailTemplateAsync("email-reset-password");

        var jsonEmailToken = JsonSerializer.Serialize(new EmailTokenDTO() { Email = email, Validity = DateTime.Now.AddHours(3) });
        var emailHash = CryptographyExtensions.Encrypt(_appSettings.Value.EmailTokenSecretKey, jsonEmailToken);

        message = message
            .Replace("{userId}", userId.ToString())
            .Replace("{emailHash}", emailHash);

        await SendEmailAsync("Sharp Template - Reset Password", message, email);
    }

    private async Task SendEmailAsync(string subject, string message, string email)
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

    private static async Task<string> LoadEmailTemplateAsync(string templateName)
    {
        using (var sr = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", $"{templateName}.html")))
        {
            return await sr.ReadToEndAsync();
        }
    }
}
