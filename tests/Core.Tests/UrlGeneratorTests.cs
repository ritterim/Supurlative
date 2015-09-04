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
            var request = WebApiHelper.GetRequest();
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

        [Fact]
        public void Make_sure_null_nested_class_property_values_do_not_show_in_url()
        {
            var request = new TestNestedClass { Id = 1 };
            var result = Generator.Generate("foo.show", request);

            Assert.Equal("http://localhost:8000/foo/1", result);
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
