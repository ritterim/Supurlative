using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Http;
using System.Web.Http.Routing;

namespace RimDev.Supurlative
{
    public static class Extensions
    {
        public static IList<string> GetNames(this HttpRouteCollection routes)
        {
            var field = typeof (HttpRouteCollection).GetField("_dictionary",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var dictionary = field.GetValue(routes) as IDictionary<string, IHttpRoute>;
            return dictionary.Keys.ToList();
        }

        public static bool CheckIfAnonymousType(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        public static IDictionary<string, object> TraverseForKeys(
            this object target,
            SupurlativeOptions options,
            string parentKey = null)
        {
            var kvp = new Dictionary<string, object>();

            if (target == null)
                return kvp;

            var valueType = target as Type == null
                ? target.GetType()
                : target as Type;

            var properties = valueType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite || valueType.CheckIfAnonymousType());

            foreach (var property in properties)
            {
                var fullPropertyName = parentKey == null
                        ? property.Name
                        : string.Format("{0}{1}{2}", parentKey, options.PropertyNameSeperator, property.Name);

                object value = null;

                if (target as Type == null)
                {
                    value = property.GetValue(target, null)
                        ?? property.PropertyType;
                }

                var formatterAttribute = property.GetCustomAttributes()
                    .Where(x => typeof(BaseFormatterAttribute).IsAssignableFrom(x.GetType()))
                    .Cast<BaseFormatterAttribute>()
                    .FirstOrDefault();

                if (formatterAttribute == null)
                {
                    // find any global formatters
                    formatterAttribute =
                        options
                        .Formatters
                        .Where(x => x.Key == property.PropertyType)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                }

                if (formatterAttribute != null)
                {
                    formatterAttribute.Invoke(fullPropertyName, value, kvp, options);
                }
                else
                {
                    if (property.PropertyType.IsPrimitive
                        || (!string.IsNullOrEmpty(property.PropertyType.Namespace)
                        && property.PropertyType.Namespace.StartsWith("System")))
                    {
                        kvp.Add(fullPropertyName, (value != null ? value.ToString() : string.Empty));
                    }
                    else
                    {
                        var results = TraverseForKeys(value, options, fullPropertyName);

                        foreach (var result in results)
                        {
                            kvp.Add(result.Key, result.Value);
                        }
                    }
                }
            }

            return kvp.ToDictionary(x => x.Key.ToLower(), x => x.Value);
        }
    }
}
