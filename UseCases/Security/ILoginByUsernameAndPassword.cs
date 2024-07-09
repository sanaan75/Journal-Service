namespace UseCases.Security;

public interface ILoginByUsernameAndPassword
{
    void Respond(string username, string planPassword);
}