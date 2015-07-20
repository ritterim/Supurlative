using Tavis.UriTemplates;
using Xunit;

namespace RimDev.Supurlative.Tests
{
    public class TemplateUrlTests
    {
        public TemplateGenerator Generator { get; set; }

        public TemplateUrlTests()
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
            var template = Generator.Generate("foo.show");

            var uriTemplate = new UriTemplate(template);

            uriTemplate.AddParameter("id", 1);

            var generatedUri = uriTemplate.Resolve();

            Assert.Equal("http://localhost:8000/foo/1", generatedUri);
        }

        [Fact]
        public void Can_generate_a_path_with_anonymous_complex_route_properties()
        {
            var template = Generator.Generate("foo.show", new { Id = 1, Bar = new { Abc = "abc", Def = "def" } });

            var uriTemplate = new UriTemplate(template);

            uriTemplate.AddParameters(new { id = 1 });
            uriTemplate.AddParameter("bar.abc", "abc");
            uriTemplate.AddParameter("bar.def", "def");

            var generatedUri = uriTemplate.Resolve();

            Assert.Equal("http://localhost:8000/foo/1?bar.abc=abc&bar.def=def", generatedUri);
        }

        [Fact]
        public void Can_generate_two_optional_path_items_template()
        {
            var template = Generator.Generate("bar.one.two", new { two = 2});

            var uriTemplate = new UriTemplate(template);

            uriTemplate.AddParameter("two", "2");

            var generatedUri = uriTemplate.Resolve();

            Assert.Equal("http://localhost:8000/bar/2", generatedUri);
        }
    }
}
