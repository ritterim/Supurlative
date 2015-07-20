using System;
using System.Collections.Generic;

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
            Formatters = new Dictionary<Type, BaseFormatterAttribute>();
        }

        public void Validate()
        {
            if (UriKind == UriKind.RelativeOrAbsolute) throw new ArgumentException("must choose between relative or absolute", "UriKind");
            if (PropertyNameSeperator == null) throw new ArgumentNullException("PropertyNameSeperator", "seperator must not be null");
        }

        public IDictionary<Type, BaseFormatterAttribute> Formatters { get; protected set; }

        public SupurlativeOptions AddFormatter(Type type, BaseFormatterAttribute formatter)
        {
            if (formatter == null) throw new ArgumentNullException("formatter");

            Formatters.Add(type, formatter);
            return this;
        }

        public SupurlativeOptions AddFormatter<T>(BaseFormatterAttribute formatter)
        {
            return AddFormatter(typeof(T), formatter);
        }

        public SupurlativeOptions AddFormatter<T>(Func<T, string> func)
        {
            return AddFormatter(typeof(T),
                new LambdaFormatter((x) => func((T)x)));
        }
    }
}
