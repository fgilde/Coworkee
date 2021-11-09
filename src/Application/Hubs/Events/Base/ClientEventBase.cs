namespace CleanArchitectureBase.Application.Hubs.Events.Base
{
    public abstract class ClientEventBase
    {
        protected ClientEventBase()
        {}

        protected ClientEventBase(EventTarget target)
        {
            Target = target;
        }

        public EventTarget Target { get; set; } = EventTarget.Current;

        public virtual string EventName => GetType().FullName;
    }
}