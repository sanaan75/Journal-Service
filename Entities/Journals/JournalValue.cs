namespace Entities.Journals;

public enum JournalValue
{
    [EnumInfo("الف")] A = 0x1,
    [EnumInfo("ب")] B = 0x2,
    [EnumInfo("ج")] C = 0x4,
    [EnumInfo("د")] D = 0x8,
    [EnumInfo("بین‌المللی")] International = 0x10
}