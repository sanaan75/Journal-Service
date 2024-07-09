namespace Entities.Journals;

public class Journal : IEntity
{
    public int Id { get; set; }
    
    public string Title { get; set; }
    public string Issn { get; set; }
    public string EIssn { get; set; }
    
    public string WebSite { get; set; }
    public string Publisher { get; set; }
    public string Country { get; set; }
    
    public ICollection<JournalRecord> Records { get; set; }
    public ICollection<BlackList> BlackLists { get; set; }
}