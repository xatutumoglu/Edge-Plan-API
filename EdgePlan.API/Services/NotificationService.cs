using EdgePlan.Data.Postgre;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Hangfire;
using EdgePlan.API.Interfaces;
using EdgePlan.API.Settings;

namespace EdgePlan.API.Services;

public class NotificationService
{
    private readonly ApplicationPostgreContext _db;
    private readonly IEmailSender _email;
    private readonly EmailOptions _opts;

    public NotificationService(ApplicationPostgreContext db, IEmailSender email, IOptions<EmailOptions> opts)
    {
        _db = db;
        _email = email;
        _opts = opts.Value;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task SendWelcomeEmailAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, ct);
        if (user == null) return;

        var subject = "Welcome to EdgePlan";
        var html = $@"
            <p>Hi {System.Net.WebUtility.HtmlEncode(user.FirstName)},</p>
            <p>Welcome to <b>EdgePlan</b>! Your account has been created successfully.</p>
            <p>Enjoy planning your goals 🎯</p>";

        await _email.SendAsync(user.Email, subject, html, ct);

        if (_opts.AdminRecipients != null)
        {
            var adminHtml = $@"
                <p>New user registered:</p>
                <ul>
                    <li>Name: {System.Net.WebUtility.HtmlEncode(user.FirstName)} {System.Net.WebUtility.HtmlEncode(user.LastName)}</li>
                    <li>Email: {System.Net.WebUtility.HtmlEncode(user.Email)}</li>
                    <li>Time (UTC): {DateTime.UtcNow:O}</li>
                </ul>";
            foreach (var admin in _opts.AdminRecipients)
            {
                await _email.SendAsync(admin, "[EdgePlan] New registration", adminHtml, ct);
            }
        }
    }
}