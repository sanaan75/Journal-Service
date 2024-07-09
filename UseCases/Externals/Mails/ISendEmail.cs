namespace UseCases.Externals.Mails;

public interface ISendEmail
{
    void Respond(Request mailRequest);

    public class Request
    {
        public string Target { get; set; }
        
        public string Subject { get; set; }
        public string Body { get; set; }
        
        public string Tag { get; set; }
    }
}