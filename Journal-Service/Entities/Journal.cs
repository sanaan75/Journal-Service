namespace Journal_Service.Entities;

public class Journal : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? NormalizedTitle { get; set; }

    public string? Issn { get; set; }
    public string? EIssn { get; set; }
    public string? Publisher { get; set; }
    public string? Country { get; set; }

    public ICollection<Category> Categories { get; set; }
}