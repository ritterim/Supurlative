using System;
using System.Net.Http;
using System.Web.Http;
using Xunit;

namespace RimDev.Supurlative.Tests
{
    public class UrlGeneratorTests
    {
        public UrlGeneratorTests()
        {
            Generator = InitializeGenerator();
        }

        private readonly UrlGenerator Generator;

        private static UrlGenerator InitializeGenerator(SupurlativeOptions options = null)
        {
            var routes = new HttpRouteCollection();
            routes.MapHttpRoute("foo.show", "foo/{id}");
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
            return new UrlGenerator(request);
        }

        [Fact]
        public void Can_generate_a_fully_qualified_path()
        {
            var expected = "http://localhost:8000/foo/1";
            var actual = Generator.Generate("foo.show", new { Id = 1 });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_path_with_anonymous_complex_route_properties()
        {
            var expected = "http://localhost:8000/foo/1?bar.abc=abc&bar.def=def";

            var actual = Generator.Generate("foo.show", new { Id = 1, Bar = new { Abc = "abc", Def = "def" } });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Cannot_generate_a_path_with_invalid_constraints()
        {
            string expected = null;
            var actual = Generator.Generate("constraint", new { Id = "abc" });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_two_optional_path_items_template()
        {
            var expected = "http://localhost:8000/bar/1";
            var actual = Generator.Generate("bar.one.two", new { one = 1 });

            Assert.Equal(expected, actual);
        }
    }
}
