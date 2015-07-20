using System;

namespace RimDev.Supurlative
{
    public class SupurlativeOptions
    {
        public static readonly SupurlativeOptions Defaults =
            new SupurlativeOptions();

        public UriKind UriKind { get; set; }
        public string PropertyNameSeperator { get; set; }

        public SupurlativeOptions()
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
