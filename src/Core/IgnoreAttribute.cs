using System;
using System.Collections.Generic;

namespace RimDev.Supurlative
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : BaseFormatterAttribute
    {
        public override void Invoke(string fullPropertyName, object value, Type valueType, IDictionary<string, object> dictionary, SupurlativeOptions options)
        {
            return;
        }

        public override bool IsMatch(Type currentType, SupurlativeOptions options)
        {
            return false;
        }
    }
}
