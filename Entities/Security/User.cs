namespace Entities.Security;

public class User : IEntity
{
    public int Id { get; set; }

    public string Username { get; set; }
    public string Password { get; set; }
    
    public UserType Type { get; set; }
    public string Title { get; set; }

    public string Email { get; set; }
    public string ConfirmCode { get; set; }
    public DateTime ConfirmCodeExpireDate { get; set; }
}