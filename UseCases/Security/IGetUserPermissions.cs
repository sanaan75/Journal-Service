using Entities.Security;

namespace UseCases.Security;

public interface IGetUserPermissions
{
    HashSet<Permission> Respond(int userId);
}