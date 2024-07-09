using System.Net.Mail;

namespace UseCases.Externals.Mails;

public interface ISmtpClientService
{
    void Send(MailMessage msg);
}