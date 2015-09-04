using System;
using System.Collections.Generic;
using System.Reflection;

namespace RimDev.Supurlative
{
    public abstract class BaseFormatterAttribute : Attribute
    {
        public abstract void Invoke(string fullPropertyName, object value, Type valueType, IDictionary<string, object> dictionary, SupurlativeOptions options);
        public abstract bool IsMatch(Type currentType, SupurlativeOptions options);

        protected virtual bool IsMatch(Type matchingType, Type currentType, SupurlativeOptions options)
        {
            if (matchingType == currentType)
                return true;

            return false;
        }
    }
}
