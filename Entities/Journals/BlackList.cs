namespace Entities.Journals;

public class BlackList : IEntity
{
    public int Id { get; set; }
    
    public int JournalId { get; set; }
    public Journal Journal { get; set; }
    
    public DateTime FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}