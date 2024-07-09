using Entities;
using UseCases.Sessions;

namespace UseCases;

public class ActorService : IActorService
{
    private readonly ISessionService _sessionService;
        
    private Actor _actor;

    public ActorService(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public void SetActor(Actor actor)
    {
        _actor = actor;
    }

    public Actor GetActor()
    {
        if (_actor != null)
            return _actor;

        _actor = _sessionService.GetOrStart<Actor>();

        return _actor;
    }
}