using System;
using System.Net.Http;
using System.Web.Http;

namespace RimDev.Supurlative.Tests
{
    public static class WebApiHelper
    {
        public static HttpRequestMessage GetRequest()
        {
            var routes = new HttpRouteCollection();

            routes.MapHttpRoute("foo.show", "foo/{id}");
            routes.MapHttpRoute("foo.one.two", "foo/{one}/{two}");
            routes.MapHttpRoute("bar.show", "bar/{id}", defaults: new { id = RouteParameter.Optional });
            routes.MapHttpRoute("bar.one.two", "bar/{one}/{two}",
                defaults: new { one = RouteParameter.Optional, two = RouteParameter.Optional });
            routes.MapHttpRoute("constraint", "constraints/{id:int}", null, new { id = @"\d+" });

            var configuration = new HttpConfiguration(routes);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost:8000/"),
                Method = HttpMethod.Get
            };

            request.SetConfiguration(configuration);

            return request;
        }
    }
}
