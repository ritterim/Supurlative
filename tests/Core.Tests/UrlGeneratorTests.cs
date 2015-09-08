using System;
using System.Net.Http;
using System.Web.Http;
using Xunit;

namespace RimDev.Supurlative.Tests
{
    public class UrlGeneratorTests
    {
        const string _baseUrl = "http://localhost:8000/";

        [Fact]
        public void Can_generate_a_fully_qualified_path()
        {
            string expected = _baseUrl + "foo/1";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateAUrlGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new { Id = 1 });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_relative_path()
        {
            string expected = "/foo/1";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateAUrlGenerator(_baseUrl, routeName, routeTemplate,
                supurlativeOptions: new SupurlativeOptions { UriKind = UriKind.Relative })
                .Generate(routeName, new { Id = 1 });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_path_with_anonymous_complex_route_properties()
        {
            string expected = _baseUrl + "foo/1?bar.abc=abc&bar.def=def";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateAUrlGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new { Id = 1, Bar = new { Abc = "abc", Def = "def" } });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Cannot_generate_a_path_with_invalid_constraints()
        {
            string expected = null;
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id:int}";
            string actual = TestHelper.CreateAUrlGenerator(_baseUrl, routeName, routeTemplate,
                routeConstraints: new { id = @"\d+" })
                .Generate(routeName, new { Id = "abc" });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_two_optional_path_items_template()
        {
            string expected = _baseUrl + "foo/1";
            const string routeName = "foo.one.two";
            const string routeTemplate = "foo/{one}/{two}";
            string actual = TestHelper.CreateAUrlGenerator(_baseUrl, routeName, routeTemplate,
                routeDefaults: new { one = RouteParameter.Optional, two = RouteParameter.Optional })
                .Generate(routeName, new { one = 1 });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_two_optional_path_items_template_two()
        {
            string expected = _baseUrl + "foo/1/2";
            const string routeName = "foo.one.two";
            const string routeTemplate = "foo/{one}/{two}";
            string actual = TestHelper.CreateAUrlGenerator(_baseUrl, routeName, routeTemplate,
                routeDefaults: new { one = RouteParameter.Optional, two = RouteParameter.Optional })
                .Generate(routeName, new { one = 1, two = 2 });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Make_sure_null_nested_class_property_values_do_not_show_in_url()
        {
            string expected = _baseUrl + "foo/1";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateAUrlGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new TestNestedClass { Id = 1 });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Make_sure_nested_class_property_values_do_show_in_url()
        {
            string expected = _baseUrl + "foo/1?filter.level=33";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateAUrlGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new TestNestedClass { Id = 1, Filter = new TestNestedClass.NestedClass { Level = 33 } });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Make_sure_generic_nested_class_property_values_do_show_in_url()
        {
            string expected = _baseUrl + "foo/1?filter.level=42";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateAUrlGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new { Id = 1, Filter = new { Level = 42 } });
            Assert.Equal(expected, actual);
        }

        public class TestNestedClass
        {
            public int Id { get; set; }

            public NestedClass Filter { get; set; }

            public class NestedClass
            {
                public int Level { get; set; }
            }
        }

    }
}
