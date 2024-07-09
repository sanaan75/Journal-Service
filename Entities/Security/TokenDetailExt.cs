namespace Entities.Security;

public static class TokenDetailExt
{
    public static IQueryable<TokenDetail> FilterByUser(this IQueryable<TokenDetail> items, int? userId)
    {
        return items.Filter(userId, x => x.UserId == userId);
    }

    public static IQueryable<TokenDetail> FilterDeactive(this IQueryable<TokenDetail> query)
    {
        return query.Where(detail => detail.IsActive == false);
    }

    public static IQueryable<TokenDetail> FilterTokensOfUser(this IQueryable<TokenDetail> items, int? userId)
    {
        return items.Filter(userId, tokenDetail => tokenDetail.UserId == userId.Value);
    }

    public static IQueryable<TokenDetail> FilterActive(this IQueryable<TokenDetail> items)
    {
        return items.Where(tokenDetail => tokenDetail.IsActive == true);
    }

    public static TokenDetail? GetByUserIdAndState(this IQueryable<TokenDetail> items, int id, string state)
    {
        return items.FirstOrDefault(detail => detail.State.Equals(state) && detail.UserId == id);
    }

    public static TokenDetail? GetByState(this IQueryable<TokenDetail> items, string state)
    {
        return items.FirstOrDefault(x => x.State.Equals(state));
    }
}