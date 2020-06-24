using AGS.API;

namespace AGS.Engine
{
    public interface IEntitySubscription
    {
        void Subscribe(IEntity entity);
        void Unsubscribe(IEntity entity);
    }
}
