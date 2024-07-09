namespace Entities.Journals;

public enum JournalIndex : byte
{
    [EnumInfo("JCR")] JCR = 2,
    [EnumInfo("Scopus")] Scopus = 4,
    [EnumInfo("Web Of Science")] WebOfScience = 8,
    [EnumInfo("ISC")] ISC = 16,
    [EnumInfo("Pubmed")] Pubmed = 32,
    [EnumInfo("Vezaratin")] Vezaratin = 64
}