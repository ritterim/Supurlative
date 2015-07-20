using System;
using System.Collections.Generic;

namespace RimDev.Supurlative
{
    public abstract class BaseFormatterAttribute : Attribute
    {
        public abstract void Invoke(string fullPropertyName, object value, IDictionary<string, object> dictionary, SupurlativeOptions options);
    }
}
