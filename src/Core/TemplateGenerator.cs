using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;

namespace RimDev.Supurlative
{
    public class TemplateGenerator
    {
        private readonly HttpRequestMessage _httpRequestMessage;
        private static readonly Regex PlaceholderRegex = new Regex(@"({(.*?)})", RegexOptions.Compiled);
        private static readonly Regex RouteConstraintCleanup = new Regex("(?<={.*?):.*?}", RegexOptions.Compiled);
        private static readonly Regex OptionalParameterCleanup = new Regex("/{/", RegexOptions.Compiled);

        public TemplateGenerator(HttpRequestMessage httpRequestMessage, UriKind uriKind = UriKind.Absolute)
        {
            if (httpRequestMessage == null) throw new ArgumentNullException("httpRequestMessage");
            if (uriKind == UriKind.RelativeOrAbsolute) throw new ArgumentException("must choose between relative or absolute", "uriKind");

            _httpRequestMessage = CloneHttpRequestMessageWithoutConstraints(httpRequestMessage);
            UriKind = uriKind;
        }

        public UriKind UriKind { get; protected set; }

        public string Generate(string routeName, object request = null)
        {
            if (routeName == null) throw new ArgumentNullException("routeName");
            
            var configuration = _httpRequestMessage.GetConfiguration();

            if (configuration == null) throw new ArgumentNullException("configuration");
            if (!configuration.Routes.ContainsKey(routeName)) throw new ArgumentOutOfRangeException("routeName", "route name does not currently exist in routes");

            var route = configuration.Routes[routeName];
            var placeHolders = PlaceholderRegex.Matches(route.RouteTemplate);

            var values = placeHolders
                .Cast<Match>()
                .ToDictionary (
                  match => match.Groups[2].Value, 
                  match => GetValueForPlaceholder(match, route)
                );

            var helper = new UrlHelper(_httpRequestMessage);
            var link = UriKind == UriKind.Relative 
                ? helper.Route(routeName, values) 
                : helper.Link(routeName, values);

            var result = HttpUtility.UrlDecode(link);

            result = OptionalParameterCleanup.Replace(result, "{/");
            result = RouteConstraintCleanup.Replace(result, "}");

            var queryStringKeys = GetQueryStringKeys(request, values.Select(x => x.Key));

            if (queryStringKeys.Any())
            {
                result = result + string.Format("{{?{0}}}",
                    string.Join(",", queryStringKeys));
            }

            return result;
        }

        private HttpRequestMessage CloneHttpRequestMessageWithoutConstraints(HttpRequestMessage httpRequestMessage)
        {
            var request = new HttpRequestMessage
            {
                Content = httpRequestMessage.Content,
                Method = httpRequestMessage.Method,
                RequestUri = httpRequestMessage.RequestUri,
                Version = httpRequestMessage.Version
            };


            var original = httpRequestMessage.GetConfiguration().Routes ;
            var modified = new HttpRouteCollection();

            var names = original.GetNames();
            foreach (var routeName in names)
            {
                var route = original[routeName];
                modified.MapHttpRoute(routeName,
                    route.RouteTemplate,
                    route.Defaults);
            }

            request.SetConfiguration(new HttpConfiguration(modified));

            return request;
        }

        private IEnumerable<string> GetQueryStringKeys(object request, IEnumerable<string> routeValues)
        {
            if (request == null)
            {
                return Enumerable.Empty<string>();
            }

            var requestProperties =
                request.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select((x => x.Name.ToLower()));

            return requestProperties.Except(routeValues, StringComparer.OrdinalIgnoreCase);
        }

        private static object GetValueForPlaceholder(Match ph, IHttpRoute route)
        {
            // if this is optional
            if (route.Defaults.Any(x => x.Key == ph.Groups[2].Value && x.Value == RouteParameter.Optional))
                return string.Format("{{/{0}}}", ph.Groups[2].Value);

            // just return the value
            return (object)ph.Value;
        }
    }
}
