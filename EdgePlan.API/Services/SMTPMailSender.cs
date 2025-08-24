using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using EdgePlan.API.Settings;
using EdgePlan.API.Interfaces;


namespace EdgePlan.API.Services;

public class SMTPMailSender : IEmailSender
{
    private readonly EmailOptions _opts;

    public SMTPMailSender(IOptions<EmailOptions> options)
    {
        _opts = options.Value;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(_opts.FromName, _opts.From));
        msg.To.Add(MailboxAddress.Parse(to));
        msg.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlBody };
        msg.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
        await client.ConnectAsync(_opts.Host, 465, SecureSocketOptions.SslOnConnect, ct);        if (!string.IsNullOrWhiteSpace(_opts.UserName))
        {
            await client.AuthenticateAsync(_opts.UserName, _opts.Password, ct);
        }
        await client.SendAsync(msg, ct);
        await client.DisconnectAsync(true, ct);
    }
}