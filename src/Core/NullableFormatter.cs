using System;
using System.Collections.Generic;
using System.Linq;

namespace RimDev.Supurlative
{
    public class NullableFormatter : BaseFormatterAttribute
    {
        public override void Invoke(string fullPropertyName, object value, Type valueType, IDictionary<string, object> dictionary, SupurlativeOptions options)
        {
            if (value == null)
            {
                dictionary.Add(fullPropertyName, null);
                return;
            }

            if (valueType.IsGenericType)
            {
                var valueGenericType = valueType.GetGenericArguments().First();

                var formatter = options.Formatters
                    .Where(x => x.GetType() != typeof(NullableFormatter))
                    .Where(x => x.IsMatch(valueGenericType, options))
                    .FirstOrDefault();

                if (formatter == null)
                {
                    dictionary.Add(fullPropertyName, value);
                }
                else
                {
                    formatter.Invoke(fullPropertyName, value, valueType, dictionary, options);
                }
            }
        }

        public override bool IsMatch(Type currentType, SupurlativeOptions options)
        {
            return currentType.IsGenericType && currentType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
