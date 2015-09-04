using System;
using System.Collections.Generic;

namespace RimDev.Supurlative
{
    public class SupurlativeOptions
    {
        public static SupurlativeOptions Defaults
        {
            get
            {
                return new SupurlativeOptions();
            }
        }

        public UriKind UriKind { get; set; }
        public string PropertyNameSeperator { get; set; }
        public bool LowercaseKeys { get; set; }

        public SupurlativeOptions()
        {
            UriKind = UriKind.Absolute;
            PropertyNameSeperator = ".";
            Formatters = new List<BaseFormatterAttribute>();
            LowercaseKeys = true;
        }

        public void Validate()
        {
            if (UriKind == UriKind.RelativeOrAbsolute) throw new ArgumentException("must choose between relative or absolute", "UriKind");
            if (PropertyNameSeperator == null) throw new ArgumentNullException("PropertyNameSeperator", "seperator must not be null");
        }

        public IList<BaseFormatterAttribute> Formatters { get; protected set; }

        public SupurlativeOptions AddFormatter(BaseFormatterAttribute formatter)
        {
            if (formatter == null) throw new ArgumentNullException("formatter");

            Formatters.Add(formatter);
            return this;
        }

        public SupurlativeOptions AddFormatter<T>()
            where T : BaseFormatterAttribute
        {
            return AddFormatter(Activator.CreateInstance<T>());
        }

        public SupurlativeOptions AddFormatter<T>(Func<T, string> func)
        {
            return AddFormatter(new LambdaFormatter(typeof(T), (x) => func((T)x)));
        }
    }
}
