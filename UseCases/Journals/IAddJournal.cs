using Entities.Journals;

namespace UseCases.Journals;

public interface IAddJournal
{
    Journal Respond(Request request);

    class Request
    {
        public string Title { get; set; }
        public string Issn { get; set; }
        public string EIssn { get; set; }
        public string WebSite { get; set; }
        public string Publisher { get; set; }
        public string Country { get; set; }
    }
}