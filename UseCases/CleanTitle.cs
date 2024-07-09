namespace UseCases;

public class CleanTitle:ICleanTitle
{
    public string Respond(string title)
    {
        return title.Trim().ToLower();
    }
}