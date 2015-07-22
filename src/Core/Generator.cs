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

            Options = generatorOptions;

            _templateGenerator = new TemplateGenerator(requestMessage, generatorOptions);
            _urlGenerator = new UrlGenerator(requestMessage, generatorOptions);
        }

        protected TemplateGenerator _templateGenerator;
        protected UrlGenerator _urlGenerator;

        public SupurlativeOptions Options { get; protected set; }

        public Result Generate(string routeName, object request)
        {
            return new Result
            {
                Template = GenerateTemplate(routeName, request),
                Url = GenerateUrl(routeName, request)
            };
        }

        public string GenerateUrl(string routeName, object request)
        {
            return _urlGenerator.Generate(routeName, request);           
        }

        public string GenerateTemplate(string routeName, object request)
        {
            return _templateGenerator.Generate(routeName, request);
        }
    }
}
