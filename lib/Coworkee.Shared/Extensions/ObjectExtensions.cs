
using System;
using System.Linq;
using System.Reflection;

namespace Coworkee.Shared.Extensions
{
    public static class ObjectExtensions
    {
        // TODO: Remove this and try to solve with Mapster or Automapper or whatever
        public static T CopyChangedValuesTo<T>(this object source, T target)
        {
            var properties = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public)
                .Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                try
                {
                    var value = prop.GetValue(source);

                    if (value != null && !value.Equals(prop.GetValue(target)) && ShouldSet(value, prop) )
                        prop.SetValue(target, value, null);
                }
                catch
                {}
            }

            return target;
        }

        private static bool ShouldSet(object value, PropertyInfo prop)
        {
            return (prop.PropertyType.IsValueType && prop.PropertyType != typeof(DateTime)) || !value.Equals(prop.PropertyType.GetDefaultValue());
        }

        private static object GetDefaultValue(this Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            return null;
        }
    }
}