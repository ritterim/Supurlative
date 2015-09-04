using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Xunit;

namespace RimDev.Supurlative.Tests
{
    public class FormattingTests
    {
        public FormattingTests()
        {
            var request = WebApiHelper.GetRequest();

            var configuration = request.GetConfiguration();
            configuration.Routes.MapHttpRoute("dictionary", "values");
            configuration.Routes.MapHttpRoute("location", "place");
            configuration.Routes.MapHttpRoute("party", "party");
            configuration.Routes.MapHttpRoute("nullable", "nullable");

            var options = SupurlativeOptions.Defaults;
            options.AddFormatter<NullableFormatter>();

            Generator = new Generator(request, options);
        }

        private readonly Generator Generator;

        [Fact]
        public void Can_generate_a_url_with_complex_object_using_formatter()
        {

            var expected = "http://localhost:8000/place?location=0-0";
            var actual = Generator.Generate("location", new LocationRequest
            {
                Location = new LocationRequest.Coordinate
                {
                    X = 0,
                    Y = 0
                }
            });

            Assert.Equal(expected, actual.Url);
        }

        [Fact]
        public void Can_generate_a_url_with_complex_object_using_dictionary_formatter()
        {
            var expectedTemplate = "http://localhost:8000/values{?one,two,three}";
            var expectedUrl = "http://localhost:8000/values?one=1&two=2&three=3";
            var actual = Generator.Generate("dictionary", new DictionaryRequest
            {
                Dictionary = new Dictionary<string, string>()
                {
                    { "one", "1" },
                    { "two", "2" },
                    { "three", "3" }
                }
            });

            Assert.Equal(expectedTemplate, actual.Template);
            Assert.Equal(expectedUrl, actual.Url);
        }

        [Fact]
        public void Can_generate_a_url_with_global_formatter()
        {
            var expected = "http://localhost:8000/party?date=07-15-2015";

            Generator
                .Options
                .AddFormatter<DateTime>(x => x.ToString("MM-dd-yyyy"));

            var actual = Generator.Generate("party", new PartyRequest
            {
                Date = new System.DateTime(2015, 7, 15)
            });

            Assert.Equal(expected, actual.Url);
        }

        [Fact]
        public void Can_handle_nullable_value_with_value()
        {
            var expected = "http://localhost:8000/nullable?age=1";

            var actual = Generator.Generate("nullable", new NullableRequest
            {
                Age = 1
            });

            Assert.Equal(expected, actual.Url);
        }

        [Fact]
        public void Can_handle_nullable_value_with_no_value()
        {
            var expected = "http://localhost:8000/nullable";

            var actual = Generator.Generate("nullable", new NullableRequest());

            Assert.Equal(expected, actual.Url);
        }

        [Fact]
        public void Should_throw_formatter_exception_if_formatter_throws_exception_during_generation()
        {
            var request = WebApiHelper.GetRequest();
            var configuration = request.GetConfiguration();

            configuration.Routes.MapHttpRoute("test", "test");

            var options = SupurlativeOptions.Defaults;

            options.AddFormatter<DummyFormatter>();

            var generator = new Generator(request, options);
            var exception = Assert.Throws<FormatterException>(
                () => generator.Generate("test", new { Id = 1 }));

            Assert.Equal(
                "There is a problem invoking the formatter: RimDev.Supurlative.Tests.DummyFormatter.",
                exception.Message);
        }
    }

    public class LocationRequest
    {
        [CoordinateFormatter]
        public Coordinate Location { get; set; }

        public class Coordinate
        {
            public int X { get; set; }
            public int Y { get; set; }

            public override string ToString()
            {
                return string.Format("{0}-{1}", X, Y);
            }
        }
    }

    public class PartyRequest
    {
        public System.DateTime Date { get; set; }
    }

    public class DictionaryRequest
    {
        [DictionaryFormatter]
        public IDictionary<string, string> Dictionary { get; set; }
    }

    public class NullableRequest
    {
        public int? Age { get; set; }
    }

    public class CoordinateFormatter : BaseFormatterAttribute
    {
        public override void Invoke(string fullPropertyName, object value, Type valueType, IDictionary<string, object> dictionary, SupurlativeOptions options)
        {
            var coordinates = value as LocationRequest.Coordinate;
            dictionary.Add(fullPropertyName, coordinates == null ? null : coordinates.ToString());
        }

        public override bool IsMatch(Type currentType, SupurlativeOptions options)
        {
            return IsMatch(typeof(LocationRequest.Coordinate), currentType, options);
        }
    }

    public class DictionaryFormatter : BaseFormatterAttribute
    {
        public override void Invoke(string fullPropertyName, object value, Type valueType, IDictionary<string, object> dictionary, SupurlativeOptions options)
        {
            var valueDictionary = value as Dictionary<string, string>;

            foreach (var kvp in valueDictionary)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }
        }

        public override bool IsMatch(Type currentType, SupurlativeOptions options)
        {
            return IsMatch(typeof(Dictionary<string, object>), currentType, options);
        }
    }

    public class DummyFormatter : BaseFormatterAttribute
    {
        public override void Invoke(string fullPropertyName, object value, Type valueType, IDictionary<string, object> dictionary, SupurlativeOptions options)
        {
            throw new NotImplementedException();
        }

        public override bool IsMatch(Type currentType, SupurlativeOptions options)
        {
            return true;
        }
    }
}
