using Entities.Security;
using UseCases.Externals.Mails;

namespace UseCases.Security.Emails;

public interface IRandomService : ISingleInstance
{
    int Next(int minValue, int maxValue);
}

public class RandomService : IRandomService
{
    private readonly Random _random = new();

    public int Next(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }
}

public class EmailConfirmCode : IEmailConfirmCode
{
    private readonly IDb _db;
    private readonly ITimeService _timeService;

    private readonly IRandomService _randomService;
    private readonly ISendEmail _sendEmail;

    public EmailConfirmCode(IDb db, ITimeService timeService, ISendEmail sendEmail, IRandomService randomService)
    {
        _db = db;
        _timeService = timeService;
        _sendEmail = sendEmail;
        _randomService = randomService;
    }

    public void Respond(int id)
    {
        var user = _db.Set<User>().GetById(id);

        user.ConfirmCode = _randomService.Next(1001, 9999).ToString();
        user.ConfirmCodeExpireDate = _timeService.Now.AddMinutes(10);

        _sendEmail.Respond(new ISendEmail.Request
        {
            Target = user.Email,
            Subject = "Jiro Banks Login Verification Code",
            Body = "Your login code is " + user.ConfirmCode + " and is valid until "+
                   user.ConfirmCodeExpireDate
        });
    }
}