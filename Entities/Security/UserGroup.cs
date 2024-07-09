namespace Entities.Security;

public class UserGroup : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
   
}