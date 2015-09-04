using System;
using System.Net.Http;
using System.Web.Http.Routing;

namespace RimDev.Supurlative
{
    public class UrlGenerator
    {
        public UrlGenerator(
            HttpRequestMessage requestMessage,
            SupurlativeOptions options = null)
        {
            if (requestMessage == null) throw new ArgumentNullException("requestMessage");

            _requestMessage = requestMessage;

            Options = options ?? SupurlativeOptions.Defaults;
            Options.Validate();
        }

        public SupurlativeOptions Options { get; protected set; }
        private readonly HttpRequestMessage _requestMessage;

        public string Generate(string routeName, object request)
        {
            var urlHelper = new UrlHelper(_requestMessage);

            var values = request.TraverseForKeys(options: Options);

            var link = Options.UriKind == UriKind.Relative
                ? urlHelper.Route(routeName, values)
                : urlHelper.Link(routeName, values);

            return link;
        }
    }
}
