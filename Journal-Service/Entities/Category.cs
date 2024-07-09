namespace Journal_Service.Entities;

public class Category : IEntity
{
    public const int ImpactFactorPrecision = 3;

    public int Id { get; set; }

    public int JournalId { get; set; }
    public Journal Journal { get; set; }

    public string Title { get; set; }
    public string? NormalizedTitle { get; set; }
    public int Year { get; set; }
    public JournalIndex Index { get; set; }
    public JournalType? Type { get; set; }
    public JournalValue? Value { get; set; }
    public JournalQRank? QRank { get; set; }
    public JournalIscClass? IscClass { get; set; }
    public decimal? If { get; set; }
    public decimal? Mif { get; set; }
    public decimal? Aif { get; set; }
    public string? Customer { get; set; }
}

public enum JournalIscClass
{
    Hasteh = 0x1,
    Avalieh = 0x2,
    Entezar = 0x4
}