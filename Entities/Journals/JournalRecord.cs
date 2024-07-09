namespace Entities.Journals;

public class JournalRecord : IEntity
{
    public const int ImpactFactorPrecision = 6;
    
    public int Id { get; set; }
    
    public int JournalId { get; set; }
    public Journal Journal { get; set; }
    
    public string Category { get; set; }
    
    public int Year { get; set; }
    public JournalIndex Index { get; set; }
    public JournalType? Type { get; set; }
    public JournalValue? Value { get; set; }
    public JournalIscClass? IscClass { get; set; }
    public JournalQRank? QRank { get; set; }
    
    public decimal? JournalIf { get; set; }
    public decimal? JournalMif { get; set; }
    public decimal? Aif { get; set; }
    
    public DateTime CreateDate { get; set; }
    public DateTime? LastUpdateDate { get; set; }
}