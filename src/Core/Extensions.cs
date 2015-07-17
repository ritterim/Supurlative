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
    }
}
