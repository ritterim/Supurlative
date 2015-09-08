using System;
using System.Net.Http;
using System.Web.Http;
using Xunit;

namespace RimDev.Supurlative.Tests
{
    public class TemplateTests
    {
        const string _baseUrl = "http://localhost:8000/";

        [Fact]
        public void Can_generate_a_fully_qualified_path()
        {
            string expected = _baseUrl + "foo/{id}";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate)
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
        public void Can_generate_a_fully_qualified_path_template_with_querystring()
        {
            string expected = _baseUrl + "foo/{id}{?bar}";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new { Id = 1, Bar = "Foo" });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_fully_qualified_path_template_with_multiple_querystring()
        {
            string expected = _baseUrl + "foo/{id}{?bar,bam}";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new { Id = 1, Bar = "Foo", Bam = 2 });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_optional_path_item_template()
        {
            string expected = _baseUrl + "foo{/id}";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate,
                routeDefaults: new { id = RouteParameter.Optional })
                .Generate(routeName);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_two_optional_path_items_template()
        {
            string expected = _baseUrl + "foo{/one}{/two}";
            const string routeName = "foo.one.two";
            const string routeTemplate = "foo/{one}/{two}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate,
                routeDefaults: new { one = RouteParameter.Optional, two = RouteParameter.Optional })
                .Generate(routeName);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_multipart_fully_qualified_path()
        {
            string expected = _baseUrl + "foo/{one}/{two}";
            const string routeName = "foo.one.two";
            const string routeTemplate = "foo/{one}/{two}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_path_with_constraints()
        {
            string expected = _baseUrl + "foo/{id}";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id:int}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate,
                routeConstraints: new { id = @"\d+" })
                .Generate(routeName);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_relative_path_with_constraints()
        {
            var expected = "/foo/{id}";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id:int}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate,
                routeConstraints: new { id = @"\d+" },
                supurlativeOptions: new SupurlativeOptions { UriKind = UriKind.Relative })
                .Generate(routeName);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_multipart_fully_qualified_path_with_constraints()
        {
            string expected = _baseUrl + "foo/{one}/{two}";
            const string routeName = "foo.one.two";
            const string routeTemplate = "foo/{one:int}/{two:int}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate,
                routeConstraints: new { one = @"\d+", two = @"\d+" })
                .Generate(routeName);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_path_with_anonymous_complex_route_properties()
        {
            string expected = _baseUrl + "foo/{id}{?bar.abc,bar.def}";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new { Id = 1, Bar = new { Abc = "abc", Def = "def" } });
            Assert.Equal(expected, actual);
        }

        private class WithInterface
        {
            public int Id { get; set; }
            public ITest<int> Test { get; set; }
        }

        public interface ITest<T>
        {
            T First { get; }
        }

        [Fact]
        public void Can_handle_open_generic_interface()
        {
            string expected = _baseUrl + "someurl/{id}{?test}";
            const string routeName = "someurl.show";
            const string routeTemplate = "someurl/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate)
                .Generate<WithInterface>(routeName);
            Assert.Equal(expected, actual);
        }

        private class ComplexRouteParameters
        {
            public ComplexRouteParameters()
            {
                Bar = new BarType();
            }

            public BarType Bar { get; set; }
            public int Id { get; set; }

            public class BarType
            {
                public string Abc { get; set; }
                public string Def { get; set; }
            }
        }

        [Fact]
        public void Can_generate_a_path_with_concrete_complex_route_properties()
        {
            string expected = _baseUrl + "someurl/{id}{?bar.abc,bar.def}";
            const string routeName = "someurl.show";
            const string routeTemplate = "someurl/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new ComplexRouteParameters());
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_path_with_concrete_complex_route_where_property_value_is_null()
        {
            string expected = _baseUrl + "someurl/{id}{?bar.abc,bar.def}";
            const string routeName = "someurl.show";
            const string routeTemplate = "someurl/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new ComplexRouteParameters() { Bar = null });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_path_with_concrete_complex_route_with_generic()
        {
            string expected = _baseUrl + "someurl/{id}{?bar.abc,bar.def}";
            const string routeName = "someurl.show";
            const string routeTemplate = "someurl/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate)
                .Generate<ComplexRouteParameters>(routeName);
            Assert.Equal(expected, actual);
        }

        private class HasIgnoredProperties
        {
            public int Id { get; set; }
            public int? Foo { get; set; }
            public string Golf { get; set; }
            // Bar and Bar2 are ignored properties
            [Supurlative.IgnoreAttribute]
            public int? Bar { get; set; }
            [Supurlative.Ignore]
            public int? Bar2 { get; set; }
        }

        [Fact]
        public void Can_generate_a_path_without_ignored_properties()
        {
            string expected = _baseUrl + "someurl/{id}{?foo,golf}";
            const string routeName = "someurl.show";
            const string routeTemplate = "someurl/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate)
                .Generate<HasIgnoredProperties>(routeName);
            Assert.Equal(expected, actual);
        }

    }
}
