using System.Net.Mail;

namespace UseCases.Externals.Mails;

public class SmtpClientService : ISmtpClientService
{
    private readonly SmtpClient _smtpClient;

    public SmtpClientService()
    {
        _smtpClient = new SmtpClient();

        throw new NotImplementedException();
        // _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        // _smtpClient.EnableSsl = true;
        // _smtpClient.Host = MailHost;
        // _smtpClient.Port = Port;
        // _smtpClient.UseDefaultCredentials = false;
        // _smtpClient.Credentials =  new NetworkCredential(MailUsername, MailPassword);
    }

    public void Send(MailMessage msg)
    {
        _smtpClient.Send(msg);
    }
}