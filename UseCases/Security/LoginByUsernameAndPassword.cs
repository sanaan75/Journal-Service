using Entities;
using Entities.Security;

namespace UseCases.Security;

public class LoginByUsernameAndPassword : ILoginByUsernameAndPassword
{
    private readonly IDb _db;
    private readonly IHashPassword _hashPassword;
    private readonly IActorService _actorService;
    private readonly IGetUserPermissions _getUserPermissions;

    public LoginByUsernameAndPassword(IDb db, 
        IActorService actorService, 
        IHashPassword hashPassword, 
        IGetUserPermissions getUserPermissions)
    {
        _db = db;
        _actorService = actorService;
        _hashPassword = hashPassword;
        _getUserPermissions = getUserPermissions;
    }

    public void Respond(string username, string planPassword)
    {
        Check.Given(username, ()=> "[Username] نامشخص است");
        Check.Given(planPassword, ()=> "[Password] نامشخص است");
        
        var hashedPassword = _hashPassword.Respond(username, planPassword);

        var user = _db.Query<User>()
            .FilterByUsername(username)
            .Select(i => new
            {
                i.Id,
                i.Type,
                i.Password
            })
            .SingleOrDefault();

        Check.NotNull(user, () => "[Username] یا [Password] اشتباه است");
        Check.Equal(hashedPassword, user.Password, () => "[Username] یا [Password] اشتباه است");

        _actorService.SetActor(new Actor
        {
            UserId = user.Id,
            Type = user.Type,
            Permissions = _getUserPermissions.Respond(user.Id)
        });
    }
}