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
    }
}
