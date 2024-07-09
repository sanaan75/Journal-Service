
namespace UseCases.Security.Emails;

public interface IEmailConfirmCode
{
    public void Respond(int userId);
}