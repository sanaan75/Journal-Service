using Entities.Security;

namespace UseCases.Security;

public class GetUserPermissions : IGetUserPermissions
{
    public HashSet<Permission> Respond(int userId)
    {
        return new HashSet<Permission>();
    }
}