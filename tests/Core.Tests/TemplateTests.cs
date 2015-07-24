using System;
using System.Net.Http;
using System.Web.Http;
using Xunit;

namespace RimDev.Supurlative.Tests
{
    public class TemplateTests
    {
        public TemplateGenerator Generator { get; set; }

        public TemplateTests()
        {
            Generator = InitializeGenerator();
        }

        private static TemplateGenerator InitializeGenerator(SupurlativeOptions options = null)
        {
            var request = WebApiHelper.GetRequest();
            return new TemplateGenerator(request, options ?? SupurlativeOptions.Defaults);
        }

        [Fact]
        public void Can_generate_a_fully_qualified_path()
        {
            var expected = "http://localhost:8000/foo/{id}";
            var actual = Generator.Generate("foo.show");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_relative_path()
        {
            var generator = InitializeGenerator(new SupurlativeOptions { UriKind = UriKind.Relative });

            var expected = "/foo/{id}";
            var actual = generator.Generate("foo.show");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_fully_qualified_path_template_with_querystring()
        {
            var expected = "http://localhost:8000/foo/{id}{?bar}";
            var actual = Generator.Generate("foo.show", new { Id = 1, Bar = "Foo" });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_fully_qualified_path_template_with_multiple_querystring()
        {
            var expected = "http://localhost:8000/foo/{id}{?bar,bam}";
            var actual = Generator.Generate("foo.show", new { Id = 1, Bar = "Foo", Bam = 2 });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_optional_path_item_template()
        {
            var expected = "http://localhost:8000/bar{/id}";
            var actual = Generator.Generate("bar.show");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_two_optional_path_items_template()
        {
            var expected = "http://localhost:8000/bar{/one}{/two}";
            var actual = Generator.Generate("bar.one.two");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_multipart_fully_qualified_path()
        {
            var expected = "http://localhost:8000/foo/{one}/{two}";
            var actual = Generator.Generate("foo.one.two");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_path_with_constraints()
        {
            var expected = "http://localhost:8000/constraints/{id}";
            var actual = Generator.Generate("constraint");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_path_with_anonymous_complex_route_properties()
        {
            var expected = "http://localhost:8000/foo/{id}{?bar.abc,bar.def}";

            var actual = Generator.Generate("foo.show", new {Id = 1, Bar = new {Abc = "abc", Def = "def"}});

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_path_with_concrete_complex_route_properties()
        {
            var expected = "http://localhost:8000/foo/{id}{?bar.abc,bar.def}";

            var actual = Generator.Generate("foo.show", new ComplexRouteParameters());

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_path_with_concrete_complex_route_where_property_value_is_null()
        {
            var expected = "http://localhost:8000/foo/{id}{?bar.abc,bar.def}";

            var actual = Generator.Generate("foo.show", new ComplexRouteParameters() { Bar = null });

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_generate_a_path_with_concrete_complex_route_with_generic()
        {
            var expected = "http://localhost:8000/foo/{id}{?bar.abc,bar.def}";

            var actual = Generator.Generate<ComplexRouteParameters>("foo.show");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Can_handle_open_generic_interface()
        {
            var expected = "http://localhost:8000/foo/{id}{?test}";

            var actual = Generator.Generate<WithInterface>("foo.show");

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

        private class WithInterface
        {
            public int Id { get; set; }
            public ITest<int> Test { get; set; }
        }

        public interface ITest<T>
        {
            T First { get; }
        }
    }
}
