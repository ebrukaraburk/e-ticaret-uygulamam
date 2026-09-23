using System.Collections.Concurrent;
using System.Reflection;

namespace MiniB2B.Helpers
{
    public static class DynamicPropertyReader
    {
        // Reflection maliyetini azaltmak için PropertyInfo'lar cache'lenir.
        private static readonly ConcurrentDictionary<(Type Type, string PropertyName), PropertyInfo?> _cache = new();

        public static object? GetValue(object entity, string propertyName)
        {
            var type = entity.GetType();
            var key = (type, propertyName);

            var prop = _cache.GetOrAdd(key, k =>
                k.Type.GetProperty(k.PropertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));

            return prop?.GetValue(entity);
        }
    }
}