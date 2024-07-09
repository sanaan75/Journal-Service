using System.Net;
using System.Net.Mail;
using System.Security;

namespace UseCases.Externals.Mails;

public class SendEmail : ISendEmail
{
    private readonly ISmtpClientService _smtpClientService;

    public SendEmail(ISmtpClientService smtpClientService)
    {
        _smtpClientService = smtpClientService;
    }

    public string MailUsername { get; set; }
    public SecureString MailPassword { get; set; }
    
    public void Respond(ISendEmail.Request request)
    {
        var msg = new MailMessage();
        msg.From = new MailAddress(MailUsername);
        msg.To.Add(new MailAddress(request.Target));
        msg.Subject = request.Subject;
        msg.IsBodyHtml = true;
        msg.Body = string.Format("<html><head></head><body><p> " + request.Body + " </p></body>");

        _smtpClientService.Send(msg);
    }
}