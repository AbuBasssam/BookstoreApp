using Application.Interfaces;
using Domain.HelperClasses;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace Application.Services;

public class EmailsService : IEmailsService
{
    #region Fields
    private readonly EmailSettings _emailSettings;
    #endregion

    #region Constructors
    // Constructor to inject EmailSettings using IOptions<T>
    /*public EmailsService(IOptions<EmailSettings> setting)
    {
        _emailSettings = setting.Value;
    }*/

    // Constructor to inject EmailSettings directly
    public EmailsService(EmailSettings emailSettings)
    {
        _emailSettings = emailSettings;
    }

    #endregion

    #region Handle Functions
    public async Task<string> SendEmail(string email, string Message, string? reason)
    {
        //sending the Message of passwordResetLink
        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_emailSettings.host, _emailSettings.port, SecureSocketOptions.StartTls);//, true
            client.Authenticate(_emailSettings.FromEmail, _emailSettings.password);
            var bodybuilder = new BodyBuilder
            {
                HtmlBody = $"{Message}",
                TextBody = "wellcome",
            };
            var message = new MimeMessage
            {
                Body = bodybuilder.ToMessageBody()
            };
            message.From.Add(new MailboxAddress("Future Team", _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress("testing", email));
            message.Subject = reason == null ? "No Submitted" : reason;
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        //end of sending email
        return "Success";
    }

    #endregion
}
