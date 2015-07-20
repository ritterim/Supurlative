using System;
using System.Collections.Generic;

namespace RimDev.Supurlative
{
    internal class LambdaFormatter : BaseFormatterAttribute
    {
        private Func<object, string> _func;

        public LambdaFormatter(Func<object,string> func)
        {
            _func = func;
        }

        public override void Invoke(string fullPropertyName, object value, IDictionary<string, object> dictionary, SupurlativeOptions options)
        {
            dictionary.Add(fullPropertyName, _func(value));
        }
    }
}
