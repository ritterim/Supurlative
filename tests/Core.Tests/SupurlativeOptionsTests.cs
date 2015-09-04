using System;
using System.Net.Http;
using System.Web.Http;
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
        public void Can_pass_through_mixed_case_keys_from_anonymous_complex_route_properties()
        {
            string expected = _baseUrl + "foo/{id}{?Bar.Abc,Bar.Def}";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate,
                supurlativeOptions: new SupurlativeOptions
                {
                    LowercaseKeys = false,
                })
                .Generate(routeName, new { Id = 1, Bar = new { Abc = "abc", Def = "def" } });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_pass_through_upper_case_keys_from_anonymous_complex_route_properties()
        {
            string expected = _baseUrl + "foo/{id}{?BAR.Abc,BAR.Def}";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate,
                supurlativeOptions: new SupurlativeOptions
                {
                    LowercaseKeys = false,
                })
                .Generate(routeName, new { Id = 1, BAR = new { Abc = "abc", Def = "def" } });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_change_keys_to_lowercase_from_anonymous_complex_route_properties()
        {
            string expected = _baseUrl + "foo/{id}{?bar.abc,bar.def}";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate,
                supurlativeOptions: new SupurlativeOptions
                {
                    LowercaseKeys = true,
                })
                .Generate(routeName, new { Id = 1, Bar = new { Abc = "abc", Def = "def" } });
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_do_custom_separator_from_anonymous_complex_route_properties()
        {
            string expected = _baseUrl + "foo/{id}{?bar:abc,bar:def}";
            const string routeName = "foo.show";
            const string routeTemplate = "foo/{id}";
            string actual = TestHelper.CreateATemplateGenerator(_baseUrl, routeName, routeTemplate,
                supurlativeOptions: new SupurlativeOptions
                {
                    PropertyNameSeperator = ":",
                })
                .Generate(routeName, new { Id = 1, Bar = new { Abc = "abc", Def = "def" } });
            Assert.Equal(expected, actual);
        }

    }
}
