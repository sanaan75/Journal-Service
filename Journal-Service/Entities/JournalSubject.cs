namespace Journal_Service.Entities;

public class JournalSubject : IEntity
{
    public int Id { get; set; }
    public int JournalId { get; set; }
    public Journal Journal { get; set; }
    public string Category { get; set; }
}