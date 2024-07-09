using Entities;

namespace UseCases;

public interface IActorService 
{
    void SetActor(Actor actor);
    Actor GetActor();
}