using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using Xunit;

namespace RimDev.Supurlative.Tests
{
    public class TemplateTests
    {
        public TemplateGenerator Generator { get; set; }

        public TemplateTests()
        {
            Generator = InitializeGenerator(UriKind.Absolute);
        }

        private static TemplateGenerator InitializeGenerator(UriKind uriKind)
        {
            var routes = new HttpRouteCollection();
            routes.MapHttpRoute("foo.show", "foo/{id}");
            routes.MapHttpRoute("foo.one.two", "foo/{one}/{two}");
            routes.MapHttpRoute("bar.show", "bar/{id}", defaults: new { id = RouteParameter.Optional });
            routes.MapHttpRoute("bar.one.two", "bar/{one}/{two}",
                defaults: new { one = RouteParameter.Optional, two = RouteParameter.Optional });
            routes.MapHttpRoute("constraint", "constraints/{id:int}", null, new { id = @"\d+"});

            var configuration = new HttpConfiguration(routes);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost:8000/"),
                Method = HttpMethod.Get
            };

            request.SetConfiguration(configuration);
            return new TemplateGenerator(request, uriKind);
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
            var generator = InitializeGenerator(UriKind.Relative);

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
    }
}
