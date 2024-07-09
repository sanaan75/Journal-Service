using Entities.Security;

namespace Entities.Conferences;

public class Conference : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? TitleEn { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public Level Level { get; set; }
    public string? Country { get; set; }
    public string? CountryEn { get; set; }
    public string? City { get; set; }
    public string? CityEn { get; set; }
    public string? Organ { get; set; }
    public string? OrganEn { get; set; }
    public Customer? Customer { get; set; }
}