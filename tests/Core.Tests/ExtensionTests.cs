using System;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.WebHost.Routing;
using System.Web.Routing;
using Xunit;

namespace RimDev.Supurlative.Tests
{
    public class ExtensionTests
    {
        [Fact]
        public void Can_get_names_from_a_HttpRouteCollection()
        {
            var routes = new HttpRouteCollection();

            routes.MapHttpRoute("foo.show", "foo/{id}");
            routes.MapHttpRoute("foo.one.two", "foo/{one}/{two}");
            routes.MapHttpRoute("bar.show", "bar/{id}", defaults: new { id = RouteParameter.Optional });
            routes.MapHttpRoute("bar.one.two", "bar/{one}/{two}",
                defaults: new { one = RouteParameter.Optional, two = RouteParameter.Optional });
            routes.MapHttpRoute("constraint", "constraints/{id:int}", null, new { id = @"\d+" });

            var names = routes.GetNames();
            Assert.True(names.Count == 5);
        }

        [Fact]
        public void Can_get_names_from_a_HostedHttpRouteCollection()
        {
            var assembly = Assembly.Load("System.Web.Http.WebHost");
            var type = assembly.GetType("System.Web.Http.WebHost.Routing.HostedHttpRouteCollection");

            var routeCollection = new RouteCollection();

            routeCollection.MapHttpRoute("one", "one");
            routeCollection.MapHttpRoute("two", "two");
            routeCollection.MapHttpRoute("three", "three");

            var hostedRoutes = Activator.CreateInstance(type, routeCollection) as HttpRouteCollection;

            var names = hostedRoutes.GetNames();
            Assert.True(names.Count == 3);
        }
    }
}
