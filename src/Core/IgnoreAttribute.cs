using System;

namespace RimDev.Supurlative
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
        public IgnoreAttribute()
        {
            Ignore = true;
        }

        public bool Ignore { get; private set; }  

        public static bool HasIgnoreAttribute(Object x, string propertyName)
        {
            var pi = x.GetType().GetProperty(propertyName);
            if (pi == null) return false;
            return Attribute.IsDefined(pi, typeof(IgnoreAttribute));
        }
    }
}
