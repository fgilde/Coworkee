using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using CleanArchitectureBase.Application.Common.Models.Identity;

namespace CleanArchitectureBase.Application.Hubs.Events.Base
{
    public class EventTarget : IEquatable<EventTarget>
    {
        private readonly string _key;
        private readonly List<string> _groups = new();

        public string[] Groups => _groups.ToArray();

        public EventTarget() // For serializers
        { }

        private EventTarget(string key)
        {
            _key = key;
        }

        private EventTarget AddGroup(params string[] groups)
        {
            _groups.AddRange(groups);
            return this;
        }
        
        #region Equality

        public bool Equals(EventTarget other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _key == other._key;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EventTarget)obj);
        }

        public override int GetHashCode()
        {
            return (_key != null ? _key.GetHashCode() : 0);
        }

        public static bool operator ==(EventTarget left, EventTarget right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EventTarget left, EventTarget right)
        {
            return !Equals(left, right);
        }

        #endregion

        public static EventTarget All = new(nameof(All));
        public static EventTarget Current = new(nameof(Current));
        public static EventTarget WithRole(params string[] role) => new EventTarget(nameof(WithRole)).AddGroup(role);
        public static EventTarget WithPermission(params string[] permission) => new EventTarget(nameof(WithPermission)).AddGroup(permission);
        public static EventTarget User(params string[] userIds) => new EventTarget(nameof(User)).AddGroup(userIds);
        public static EventTarget User(params ClaimsPrincipal[] users) => User(users.Select(user => user.FindFirstValue(ClaimTypes.NameIdentifier)).ToArray());
        public static EventTarget User(params UserResponse[] users) => User(users.Select(user => user.Id).ToArray());
    }
}