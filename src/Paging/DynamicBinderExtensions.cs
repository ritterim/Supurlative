using System;

namespace RimDev.Supurlative.Paging
{
    internal static class DynamicBinderExtensions
    {
        internal static bool Has<TType, TReturn>(this TType target, Func<TType, TReturn> test)
        {
            try
            {
                var value = test(target);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}