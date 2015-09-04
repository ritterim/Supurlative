using System;
using System.Collections.Generic;

namespace RimDev.Supurlative
{
    internal class LambdaFormatter : BaseFormatterAttribute
    {
        private readonly Type _type;
        private Func<object, string> _func;

        public LambdaFormatter(Type type, Func<object,string> func)
        {
            _type = type;
            _func = func;
        }

        public override void Invoke(string fullPropertyName, object value, Type valueType, IDictionary<string, object> dictionary, SupurlativeOptions options)
        {
            dictionary.Add(fullPropertyName, _func(value));
        }

        public override bool IsMatch(Type currentType, SupurlativeOptions options)
        {
            return IsMatch(_type, currentType, options);
        }
    }
}
