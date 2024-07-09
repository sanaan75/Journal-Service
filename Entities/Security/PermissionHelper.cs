namespace Entities.Security;

public static class PermissionHelper
{
    public static IList<Permission> GetPermissionTree(Permission? root)
    {
        var items = EnumHelper.GetAllItems<Permission>().AsEnumerable();

        if (root.HasValue)
        {
            var rootFullValue = root.Value.GetFullValue();
            items = items.Where(i => i.GetFullValue().StartsWith(rootFullValue));
        }

        return items.OrderBy(i => i.GetFullValue()).ToList();
    }

    public static string GetFullCaption(this Permission permission)
    {
        var value = permission.ToNullableInt32().ToString();

        if (value.Length % 2 == 1)
            value = "0" + value;

        var items = new List<string>();
        var bar = "";
        for (var i = 0; i < value.Length; i = i + 2)
        {
            bar = bar + value[i] + value[i + 1];
            var c = EnumHelper.Parse<Permission>(bar.ToInt32());
            items.Add(c.GetCaption());
        }

        return StringHelper.Join(" » ", items);
    }

    public static string GetFullValue(this Permission permission)
    {
        var value = permission.ToNullableInt32();
        var valueStr = value.ToString();

        var valueLen = valueStr.Length;
        var isOdd = valueLen % 2 == 1;

        if (isOdd)
            valueStr = "0" + valueStr;

        return valueStr;
    }
}