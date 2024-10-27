namespace Journal_Service.Entities;

public class User : IEntity
{
    public int Id { get; set; }

    public string Username { get; set; }
    public string Password { get; set; }

    public string Title { get; set; }

    public bool Enabled { get; set; }
    public bool SysAdmin { get; set; }
    public string Customer { get; set; }
}