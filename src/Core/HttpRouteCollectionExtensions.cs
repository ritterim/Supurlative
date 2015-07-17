using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Routing;

namespace RimDev.Supurlative
{
    public static class HttpRouteCollectionExtensions
    {
        public static IList<string> GetNames(this HttpRouteCollection routes)
        {
            var field = typeof (HttpRouteCollection).GetField("_dictionary",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var dictionary = field.GetValue(routes) as IDictionary<string, IHttpRoute>;
            return dictionary.Keys.ToList();
        } 
    }
}
