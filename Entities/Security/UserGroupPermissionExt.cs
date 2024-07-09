namespace Entities.Security;

public static class UserGroupPermissionExt
{
    public static IQueryable<UserGroupPermission> FilterByUserGroup(this IQueryable<UserGroupPermission> items, int? id)
    {
        return items.Filter(id, i => i.UserGroupId == id.Value);
    }

    public static IQueryable<UserGroupPermission> FilterByPermission(this IQueryable<UserGroupPermission> items,
        Permission? value)
    {
        return items.Filter(value, i => i.Permission == value.Value);
    }
}