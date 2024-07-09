using System.Text.RegularExpressions;

namespace UseCases;

public class CleanISSN : ICleanISSN
{
    public string Respond(string issn)
    {
        if (string.IsNullOrWhiteSpace(issn))
            return string.Empty;

        return Regex.Replace(issn.Trim(), "[- ]", String.Empty);
    }
}