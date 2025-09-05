using lib.Coworkee.Application.Hubs.Events.Base;

namespace Coworkee.Application.Hubs.Events
{
    public class BeforeRequest<TRequest> : ClientEventBase
    {
        public BeforeRequest()
        {}

        public BeforeRequest(TRequest request) 
            : base(EventTarget.Current)
        {
            Request = request;
        }

        public string RequestName { get; } = typeof(TRequest).FullName;
        public TRequest Request { get; }
    }
}