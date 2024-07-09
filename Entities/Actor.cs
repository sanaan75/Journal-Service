using Entities.Security;

namespace Entities;

public class Actor
{
    public int UserId { get; set; }
    public UserType Type { get; set; }
    public HashSet<Permission> Permissions { get; set; }
}