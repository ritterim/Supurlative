using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Xunit;

namespace RimDev.Supurlative.Tests
{
    public class FormattingTests
    {
        const string _baseUrl = "http://localhost:8000/";

        [Fact]
        public void Can_generate_a_url_with_complex_object_using_formatter()
        {
            string expectedUrl = _baseUrl + "place?location=0-0";
            const string routeName = "location";
            const string routeTemplate = "place";

            Result actual = TestHelper.CreateAGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new LocationRequest
                {
                    Location = new LocationRequest.Coordinate
                    {
                        X = 0,
                        Y = 0
                    }
                });
            Assert.Equal(expectedUrl, actual.Url);
        }

        [Fact]
        public void Can_generate_a_url_with_complex_object_using_dictionary_formatter()
        {
            var expectedTemplate = _baseUrl + "values{?one,two,three}";
            var expectedUrl = _baseUrl + "values?one=1&two=2&three=3";
            const string routeName = "dictionary";
            const string routeTemplate = "values";

            Result actual = TestHelper.CreateAGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new DictionaryRequest
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
            string expectedUrl = _baseUrl + "party?date=07-15-2015";
            const string routeName = "party";
            const string routeTemplate = "party";

            Generator generator = TestHelper.CreateAGenerator(_baseUrl, routeName, routeTemplate);
            generator.Options.AddFormatter<DateTime>(x => x.ToString("MM-dd-yyyy"));
            Result actual = generator
                .Generate(routeName, new PartyRequest
                {
                    Date = new System.DateTime(2015, 7, 15)
                });

            Assert.Equal(expectedUrl, actual.Url);
        }

        [Fact]
        public void Can_handle_nullable_value_with_value()
        {
            string expectedUrl = _baseUrl + "nullable?age=1";
            const string routeName = "nullable";
            const string routeTemplate = "nullable";

            Result actual = TestHelper.CreateAGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new NullableRequest
                {
                    Age = 1
                });

            Assert.Equal(expectedUrl, actual.Url);
        }

        [Fact]
        public void Can_handle_nullable_value_with_no_value()
        {
            string expectedUrl = _baseUrl + "nullable";
            const string routeName = "nullable";
            const string routeTemplate = "nullable";

            Result actual = TestHelper.CreateAGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new NullableRequest());

            Assert.Equal(expectedUrl, actual.Url);
        }

        [Fact]
        public void Should_throw_formatter_exception_if_formatter_throws_exception_during_generation()
        {
            const string routeName = "test";
            const string routeTemplate = "test";
            SupurlativeOptions options = SupurlativeOptions.Defaults;
            options.AddFormatter<DummyFormatter>();

            HttpRequestMessage request;
            request = TestHelper.CreateAHttpRequestMessage(_baseUrl, routeName, routeTemplate);

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
