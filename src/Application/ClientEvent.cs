using System;
using MediatR;
using Newtonsoft.Json;

namespace CleanArchitectureBase.Application
{
    public class ClientEvent : INotification, ICloneable
    {
        public ClientEvent(TargetClient target, string eventName, object arguments = null)
        {
            Target = target;
            EventName = eventName;
            Arguments = arguments;
        }

        public TargetClient Target { get; set; }
        public string EventName { get; set; }
        public object Arguments { get; set; }
        public ClientEvent Clone()
        {
            var args = Arguments != null && !Arguments.GetType().IsPrimitive ? JsonConvert.SerializeObject(Arguments) : Arguments;
            return new ClientEvent(Target, EventName, args);
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
    }

    public enum TargetClient
    {
        All,
        Current
    }
}
