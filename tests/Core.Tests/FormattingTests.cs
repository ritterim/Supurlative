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

            var options = SupurlativeOptions.Defaults;
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

    public class CoordinateFormatter : BaseFormatterAttribute
    {
        public override void Invoke(string fullPropertyName, object value, IDictionary<string, object> dictionary, SupurlativeOptions options)
        {
            var coordinates = value as LocationRequest.Coordinate;
            dictionary.Add(fullPropertyName, coordinates == null ? null : coordinates.ToString());
        }
    }

    public class DictionaryFormatter : BaseFormatterAttribute
    {
        public override void Invoke(string fullPropertyName, object value, IDictionary<string, object> dictionary, SupurlativeOptions options)
        {
            var valueDictionary = value as Dictionary<string, string>;

            foreach (var kvp in valueDictionary)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }
        }
    }
}
