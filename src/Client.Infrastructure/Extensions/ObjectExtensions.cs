using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Infrastructure.Extensions
{
    public static class ObjectExtensions
    {
        public static string AsGet(this object obj)
        {
            return obj.ToQueryString("?").Replace(new [] { "%5b", "%22", "%5d" }, string.Empty); 
            // TODO: Remove replace, fix ToQueryString with array in Nextended.Core
        }
    }
}