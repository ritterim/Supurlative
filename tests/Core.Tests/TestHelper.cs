using System;
using System.Net.Http;
using System.Web.Http;

namespace RimDev.Supurlative.Tests
{
    public static class TestHelper
    {
        public static HttpRequestMessage CreateAHttpRequestMessage(
            string baseUrl,
            string routeName,
            string routeTemplate,
            object routeDefaults = null,
            object routeConstraints = null
            )
        {
            HttpRouteCollection routes = new HttpRouteCollection();
            routes.MapHttpRoute(
                routeName,
                routeTemplate,
                defaults: routeDefaults,
                constraints: routeConstraints
                );
            HttpConfiguration configuration = new HttpConfiguration(routes);
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri(baseUrl),
                Method = HttpMethod.Get
            };
            request.SetConfiguration(configuration);
            return request;
        }

        public static TemplateGenerator CreateATemplateGenerator(
            string baseUrl,
            string routeName,
            string routeTemplate,
            object routeDefaults = null,
            object routeConstraints = null,
            SupurlativeOptions supurlativeOptions = null
            )
        {
            HttpRequestMessage request;
            request = CreateAHttpRequestMessage(baseUrl, routeName, routeTemplate, routeDefaults, routeConstraints);
            return new TemplateGenerator(request, supurlativeOptions);
        }

        public static UrlGenerator CreateAUrlGenerator(
            string baseUrl,
            string routeName,
            string routeTemplate,
            object routeDefaults = null,
            object routeConstraints = null,
            SupurlativeOptions supurlativeOptions = null
            )
        {
            HttpRequestMessage request;
            request = CreateAHttpRequestMessage(baseUrl, routeName, routeTemplate, routeDefaults, routeConstraints);
            return new UrlGenerator(request, supurlativeOptions);
        }

        public static Generator CreateAGenerator(
            string baseUrl,
            string routeName,
            string routeTemplate,
            object routeDefaults = null,
            object routeConstraints = null,
            SupurlativeOptions supurlativeOptions = null
            )
        {
            HttpRequestMessage request;
            request = CreateAHttpRequestMessage(baseUrl, routeName, routeTemplate, routeDefaults, routeConstraints);
            return new Generator(request, supurlativeOptions);
        }
    }
}
