using System;
using System.Net.Http;

namespace RimDev.Supurlative
{
    public class Generator
    {
        public Generator(
            HttpRequestMessage requestMessage,
            SupurlativeOptions options = null)
        {
            if (requestMessage == null) throw new ArgumentNullException("requestMessage");

            var generatorOptions = options ?? SupurlativeOptions.Defaults;
            generatorOptions.Validate();

            _templateGenerator = new TemplateGenerator(requestMessage, generatorOptions);
            _urlGenerator = new UrlGenerator(requestMessage, generatorOptions);
        }

        protected TemplateGenerator _templateGenerator;
        protected UrlGenerator _urlGenerator;

        public Result Generate(string routeName, object request)
        {
            var template = _templateGenerator.Generate(routeName, request);
            var url = _urlGenerator.Generate(routeName, request);

            return new Result
            {
                Template = template,
                Url = url
            };
        }
    }
}
