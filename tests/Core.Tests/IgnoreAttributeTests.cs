﻿using System;
using Xunit;

namespace RimDev.Supurlative.Tests
{
    public class IgnoreAttributeTests
    {
        private class HasIgnoredParameters
        {
            public HasIgnoredParameters()
            {
                Id = 3;
                Foo = null;
                Bar = 300;
            }

            public int Id { get; set; }
            public int? Foo { get; set; }

            // Two ways of using the ignore attribute
            [Supurlative.IgnoreAttribute]
            public int? Bar { get; set; }
            [Supurlative.Ignore]
            public int? Bar2 { get; set; }
        }

        [Fact]
        public void Verify_that_Foo_is_visible()
        {
            HasIgnoredParameters _sut = new HasIgnoredParameters();
            var t = typeof(HasIgnoredParameters);
            var pi = t.GetProperty("Foo");
            var hasIgnoreAttribute = Attribute.IsDefined(pi, typeof(IgnoreAttribute));
            Assert.False(hasIgnoreAttribute);
        }

        [Fact]
        public void Verify_that_Id_is_visible()
        {
            HasIgnoredParameters _sut = new HasIgnoredParameters();
            var t = typeof(HasIgnoredParameters);
            var pi = t.GetProperty("Id");
            var hasIgnoreAttribute = Attribute.IsDefined(pi, typeof(IgnoreAttribute));
            Assert.False(hasIgnoreAttribute);
        }

        [Fact]
        public void Verify_that_Bar_is_ignored()
        {
            HasIgnoredParameters _sut = new HasIgnoredParameters();
            var t = typeof(HasIgnoredParameters);
            var pi = t.GetProperty("Bar");
            var hasIgnoreAttribute = Attribute.IsDefined(pi, typeof(IgnoreAttribute));
            Assert.True(hasIgnoreAttribute);
        }

        [Fact]
        public void Verify_that_Bar2_is_ignored()
        {
            HasIgnoredParameters _sut = new HasIgnoredParameters();
            var t = typeof(HasIgnoredParameters);
            var pi = t.GetProperty("Bar2");
            var hasIgnoreAttribute = Attribute.IsDefined(pi, typeof(IgnoreAttribute));
            Assert.True(hasIgnoreAttribute);
        }

        //[Fact]
        //public void Can_ignore_property()
        //{
        //    HasIgnoredParameters _sut = new HasIgnoredParameters();

        //}

        //[Fact]
        //public void Verify_that_ignore_defaults_to_false()
        //{
        //    HasIgnoredParameters _sut = new HasIgnoredParameters();

        //}

    }
}