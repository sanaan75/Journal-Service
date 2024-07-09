namespace Entities.Journals;

public enum JournalIscClass
{
    [EnumInfo("در دسته هسته")] Hasteh = 0x1,
    [EnumInfo("در دسته اولیه")] Avalieh = 0x2,
    [EnumInfo("در دسته انتظار")] Entezar = 0x4
}