using System.Web.Http;
using Tavis.UriTemplates;
using Xunit;

namespace RimDev.Supurlative.Tests
{
    public class TemplateUrlTests
    {
        const string _baseUrl = "http://localhost:8000/";

        [Fact]
        public void Can_generate_a_fully_qualified_path()
        {
            string expected = _baseUrl + "foo/1";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";

            string template = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName);
            var uriTemplate = new UriTemplate(template);
            uriTemplate.AddParameter("id", 1);

            string actual = uriTemplate.Resolve();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_path_with_anonymous_complex_route_properties()
        {
            string expected = _baseUrl + "foo/1?bar.abc=abc&bar.def=def";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";

            string template = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new { Id = 1, Bar = new { Abc = "abc", Def = "def" } });
            var uriTemplate = new UriTemplate(template);

            uriTemplate.AddParameters(new { id = 1 });
            uriTemplate.AddParameter("bar.abc", "abc");
            uriTemplate.AddParameter("bar.def", "def");

            string actual = uriTemplate.Resolve();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_two_optional_path_items_template()
        {
            string expected = _baseUrl + "foo/2";
            const string routeName = "foo.one.two";
            const string routeTemplate = "foo/{one}/{two}";

            string template = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate,
                routeDefaults: new { one = RouteParameter.Optional, two = RouteParameter.Optional })
                .Generate(routeName, new { two = 2 });
            var uriTemplate = new UriTemplate(template);

            uriTemplate.AddParameter("two", "2");

            string actual = uriTemplate.Resolve();
            Assert.Equal(expected, actual);
        }
    }
}
