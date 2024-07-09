namespace Entities.Journals;

public static class JournalExt
{
    public static IQueryable<Journal> FilterByKeyword(this IQueryable<Journal> items, string keyword)
    {
        var tokens = keyword.GetTokens();

        foreach (var token in tokens) 
            items = items.Where(i => i.Title.Contains(token) || i.Issn.Contains(token));

        return items;
    }

    public static IQueryable<Journal> FilterByIssn(this IQueryable<Journal> items, string issn)
    {
        return items.Filter(issn, i => i.Issn.Trim().Equals(issn.Trim()));
    }

    public static IQueryable<Journal> FilterByEIssn(this IQueryable<Journal> items, string eIssn)
    {
        return items.Filter(eIssn, i => i.EIssn.Trim().Equals(eIssn.Trim()));
    }

    public static IQueryable<Journal> FilterByTitle(this IQueryable<Journal> items, string title)
    {
        return items.Filter(title, i => i.Title.Trim().ToLower().Equals(title.Trim().ToLower()));
    }

    public static IQueryable<Journal> FilterByIndex(this IQueryable<Journal> items, JournalIndex? index)
    {
        return items.Filter(index, i => i.Records.Any(r => r.Index == index.Value));
    }

    public static IQueryable<Journal> FilterByQRank(this IQueryable<Journal> items, JournalQRank? QRank)
    {
        return items.Filter(QRank, i => i.Records.Any(r => r.QRank.Value == QRank.Value));
    }

    public static IQueryable<Journal> FilterByYear(this IQueryable<Journal> items, int? year)
    {
        return items.Filter(year, i => i.Records.Any(r => r.Year == year.Value));
    }
}