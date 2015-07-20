using System;

namespace RimDev.Supurlative
{
    public class TemplateGeneratorOptions
    {
        public static readonly TemplateGeneratorOptions Defaults =
            new TemplateGeneratorOptions();

        public UriKind UriKind { get; set; }
        public string PropertyNameSeperator { get; set; }

        public TemplateGeneratorOptions()
        {
            UriKind = UriKind.Absolute;
            PropertyNameSeperator = ".";
        }

        public void Validate()
        {
            if (UriKind == UriKind.RelativeOrAbsolute) throw new ArgumentException("must choose between relative or absolute", "UriKind");
            if (PropertyNameSeperator == null) throw new ArgumentNullException("PropertyNameSeperator", "seperator must not be null");
        }
    }
}
