using System;
using Coworkee.Application.Hubs.Events.Base;
using MediatR;
using Nextended.Core.Extensions;

namespace Coworkee.Application.Hubs
{
    public class ClientEventNotification: INotification, ICloneable
    {
        public ClientEventNotification(ClientEventBase clientEvent)
        {
            Target = clientEvent.Target;
            EventName = clientEvent.EventName;
            Arguments = clientEvent;
        }

        public ClientEventNotification(ClientEventBase clientEvent, EventTarget targetOverwrite) 
            : this(clientEvent)
        {
            Target = targetOverwrite;
        }

        public EventTarget Target { get; set; }
        public string EventName { get; set; }
        public object Arguments { get; set; }

        public ClientEventNotification Clone()
        {
            return this.MapTo<ClientEventNotification>();
        }
        
        object ICloneable.Clone() => Clone();
    }
}
