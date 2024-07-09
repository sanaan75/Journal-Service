namespace Entities.Security;

public static class UserExt
{
    public static IQueryable<User> FilterByUsername(this IQueryable<User> items, string username)
    {
        return items.Filter(username, i => i.Username == username);
    }
}