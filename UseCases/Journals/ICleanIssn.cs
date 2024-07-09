namespace UseCases.Journals;

public interface ICleanIssn
{
    string Respond(string issn);
}

public class CleanIssn : ICleanIssn
{
    public string Respond(string issn)
    {
        throw new NotImplementedException();
    }
}