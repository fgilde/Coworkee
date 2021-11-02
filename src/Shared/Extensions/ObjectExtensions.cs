
using System.Linq;
using System.Reflection;

namespace CleanArchitectureBase.Shared.Extensions
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
                var value = prop.GetValue(source);
                if (value != null && !value.Equals(prop.GetValue(target)))
                    prop.SetValue(target, value, null);
            }

            return target;
        }
    }
}