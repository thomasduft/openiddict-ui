using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace tomware.Microip.Web
{
  public class LogEmailSender : IEmailSender
  {
    private readonly ILogger<LogEmailSender> logger;

    public LogEmailSender(
      ILogger<LogEmailSender> logger
    )
    {
      this.logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
      this.logger.LogInformation(
        "Sending email to '{Recipient}' with '{subject}' and '{Content}'",
        email,
        subject,
        htmlMessage
      );

      await Task.CompletedTask;
    }
  }
}
