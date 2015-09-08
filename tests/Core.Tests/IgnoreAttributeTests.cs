using System;
using Xunit;

namespace RimDev.Supurlative.Tests
{
    public class IgnoreAttributeTests
    {
        private class HasIgnoredProperties
        {
            public HasIgnoredProperties()
            {
                Id = 3;
                Foo = null;
                Bar = 300;
                Bar2 = null;
            }

            public int Id { get; set; }
            public int? Foo { get; set; }

            // Two ways of using the ignore attribute (with and without Attribute suffix)
            [Supurlative.IgnoreAttribute]
            public int? Bar { get; set; }
            [Supurlative.Ignore]
            public int? Bar2 { get; set; }
        }

        [Fact]
        public void Verify_that_Foo_is_visible()
        {
            HasIgnoredProperties _sut = new HasIgnoredProperties();
            var t = typeof(HasIgnoredProperties);
            var pi = t.GetProperty("Foo");
            bool hasIgnoreAttribute = Attribute.IsDefined(pi, typeof(IgnoreAttribute));
            Assert.False(hasIgnoreAttribute);
        }

        [Fact]
        public void Verify_that_Id_is_visible()
        {
            HasIgnoredProperties _sut = new HasIgnoredProperties();
            var t = typeof(HasIgnoredProperties);
            var pi = t.GetProperty("Id");
            bool hasIgnoreAttribute = Attribute.IsDefined(pi, typeof(IgnoreAttribute));
            Assert.False(hasIgnoreAttribute);
        }

        [Fact]
        public void Verify_that_Bar_is_ignored()
        {
            HasIgnoredProperties _sut = new HasIgnoredProperties();
            var t = typeof(HasIgnoredProperties);
            var pi = t.GetProperty("Bar");
            bool hasIgnoreAttribute = Attribute.IsDefined(pi, typeof(IgnoreAttribute));
            Assert.True(hasIgnoreAttribute);
        }

        [Fact]
        public void Verify_that_Bar2_is_ignored()
        {
            HasIgnoredProperties _sut = new HasIgnoredProperties();
            var t = typeof(HasIgnoredProperties);
            var pi = t.GetProperty("Bar2");
            bool hasIgnoreAttribute = Attribute.IsDefined(pi, typeof(IgnoreAttribute));
            Assert.True(hasIgnoreAttribute);
        }

    }
}
