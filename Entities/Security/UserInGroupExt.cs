namespace Entities.Security;

public static class UserInGroupExt
{
    public static IQueryable<UserInGroup> FilterByUser(this IQueryable<UserInGroup> items, int? id)
    {
        return items.Filter(id, i => i.UserId == id.Value);
    }

    public static IQueryable<UserInGroup> FilterByUserGroup(this IQueryable<UserInGroup> items, int? id)
    {
        return items.Filter(id, i => i.UserGroupId == id.Value);
    }
}