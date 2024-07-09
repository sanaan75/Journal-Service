namespace Entities.Security;

public class UserGroupPermission : IEntity
{
    public int Id { get; set; }
    public int UserGroupId { get; set; }
    public UserGroup UserGroup { get; set; }
    public Permission Permission { get; set; }
}