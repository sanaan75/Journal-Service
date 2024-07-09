namespace Entities.Security;

public class UserInGroup : IEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int UserGroupId { get; set; }
    public UserGroup UserGroup { get; set; }
}