using System;
using System.Net.Http;
using System.Web.Http;
using Tavis.UriTemplates;
using Xunit;

namespace RimDev.Supurlative.Tests
{
    public class SupurlativeOptionsTests
    {
        const string _baseUrl = "http://localhost:8000/";

        [Fact]
        public void Can_generate_an_absolute_path()
        {
            string expected = _baseUrl + "foo/{id}";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate,
                supurlativeOptions: new SupurlativeOptions { UriKind = UriKind.Absolute })
                .Generate(routeName);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_relative_path()
        {
            var expected = "/foo/{id}";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate, 
                supurlativeOptions: new SupurlativeOptions { UriKind = UriKind.Relative })
                .Generate(routeName);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_passthrough_uppercase_keys()
        {
            string expected = _baseUrl + "foo/1?Bar.Abc=abc&Bar.Def=def";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";

            string template = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate,
                supurlativeOptions: new SupurlativeOptions
                {
                    LowercaseKeys = false,
                })
                .Generate(routeName, new { Id = 1, Bar = new { Abc = "abc", Def = "def" } });
            var uriTemplate = new UriTemplate(template);

            uriTemplate.AddParameters(new { id = 1 });
            uriTemplate.AddParameter("Bar.Abc", "abc");
            uriTemplate.AddParameter("Bar.Def", "def");

            string actual = uriTemplate.Resolve();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_change_keys_to_lowercase()
        {
            string expected = _baseUrl + "foo/1?bar.abc=abc&bar.def=def";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";

            string template = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate,
                supurlativeOptions: new SupurlativeOptions
                {
                    LowercaseKeys = true,
                })
                .Generate(routeName, new { Id = 1, Bar = new { Abc = "abc", Def = "def" } });
            var uriTemplate = new UriTemplate(template);

            uriTemplate.AddParameters(new { id = 1 });
            uriTemplate.AddParameter("Bar.Abc", "abc");
            uriTemplate.AddParameter("Bar.Def", "def");

            string actual = uriTemplate.Resolve();
            Assert.Equal(expected, actual);
        }

    }
}
