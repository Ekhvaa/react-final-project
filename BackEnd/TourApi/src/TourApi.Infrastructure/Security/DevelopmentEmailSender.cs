using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TourApi.Common;
using TourApi.Services;

namespace TourApi.Infrastructure.Security;

public sealed class DevelopmentEmailSender : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly ILogger<DevelopmentEmailSender> _logger;

    public DevelopmentEmailSender(IOptions<EmailOptions> options, ILogger<DevelopmentEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task SendEmailConfirmationAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var link = $"{_options.FrontendBaseUrl.TrimEnd('/')}/confirm-email?token={Uri.EscapeDataString(token)}";
        _logger.LogInformation("Email confirmation for {Email}: {Link}", email, link);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var link = $"{_options.FrontendBaseUrl.TrimEnd('/')}/reset-password?token={Uri.EscapeDataString(token)}";
        _logger.LogInformation("Password reset for {Email}: {Link}", email, link);
        return Task.CompletedTask;
    }
}
