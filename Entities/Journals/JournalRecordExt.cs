namespace Entities.Journals;

public static class JournalRecordExt
{
    public static IQueryable<JournalRecord> FilterByJournal(this IQueryable<JournalRecord> items, int? id)
    {
        return items.Filter(id, i => i.JournalId == id.Value);
    }

    public static IQueryable<JournalRecord> FilterByJournalTitle(this IQueryable<JournalRecord> items, string title)
    {
        return items.Filter(title, i => i.Journal.Title.Trim().ToLower().Contains(title.Trim().ToLower()));
    }

    public static IQueryable<JournalRecord> FilterByCategory(this IQueryable<JournalRecord> items, string category)
    {
        return items.Filter(category, i => i.Category.Trim().ToLower().Contains(category.Trim().ToLower()));
    }

    public static IQueryable<JournalRecord> FilterByIndex(this IQueryable<JournalRecord> items, JournalIndex? index)
    {
        return items.Filter(index, i => i.Index == index);
    }

    public static IQueryable<JournalRecord> FilterByIssn(this IQueryable<JournalRecord> items, string issn)
    {
        return items.Filter(issn, i => i.Journal.Issn.Replace("-", "").Trim().Equals(issn.Replace("-", "").Trim()));
    }

    public static IQueryable<JournalRecord> FilterByJournalType(this IQueryable<JournalRecord> items,
        JournalType? journalType)
    {
        return items.Filter(journalType, i => i.Type == journalType);
    }

    public static IQueryable<JournalRecord> FilterByIf(this IQueryable<JournalRecord> items, decimal? from,
        decimal? to)
    {
        items = items.Filter(from, i => i.JournalIf >= from);
        return items.Filter(to, i => i.JournalIf <= to);
    }

    public static IQueryable<JournalRecord> FilterByMif(this IQueryable<JournalRecord> items, decimal? from,
        decimal? to)
    {
        items = items.Filter(from, i => i.JournalMif >= from);
        return items.Filter(to, i => i.JournalMif <= to);
    }

    public static IQueryable<JournalRecord> FilterByAif(this IQueryable<JournalRecord> items, decimal? from,
        decimal? to)
    {
        items = items.Filter(from, i => i.Aif >= from);
        return items.Filter(to, i => i.Aif <= to);
    }

    public static IQueryable<JournalRecord> FilterByYear(this IQueryable<JournalRecord> items, int? from, int? to)
    {
        items = items.Filter(from, i => i.Year >= from);
        return items.Filter(to, i => i.Year <= to);
    }

    public static IQueryable<JournalRecord> FilterByYear(this IQueryable<JournalRecord> items, int? year)
    {
        return items.Filter(year, i => i.Year == year.Value);
    }
}