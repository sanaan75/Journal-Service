namespace Entities.Conferences;

public static class ConferenceExt
{
    public static IQueryable<Conference> FilterByKeyword(this IQueryable<Conference> items, string keyword)
    {
        var tokens = keyword.GetTokens();

        foreach (var token in tokens)
            items = items.Where(i => i.Title.Contains(token) || i.TitleEn.Contains(token));

        return items;
    }

    public static IQueryable<Conference> FilterByTitle(this IQueryable<Conference> items, string title)
    {
        return items.Where(i => i.Title.Trim().ToLower().Equals(title.Trim().ToLower()));
    }

    public static IQueryable<Conference> FilterByTitleEn(this IQueryable<Conference> items, string title)
    {
        return items.Where(i => i.TitleEn.Trim().ToLower().Equals(title.Trim().ToLower()));
    }
}