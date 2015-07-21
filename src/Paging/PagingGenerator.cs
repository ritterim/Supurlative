using PagedList;
using System;
using System.Net.Http;

namespace RimDev.Supurlative.Paging
{
    public class PagingGenerator : UrlGenerator
    {
        public PagingGenerator(
            HttpRequestMessage requestMessage,
            SupurlativeOptions options = null,
            string pagePropertyName = "page")
            : base(requestMessage, options)
        {
            PagePropertyName = pagePropertyName;
        }

        public string PagePropertyName { get; protected set; }

        public PagingResult Generate(string routeName, object request, IPagedList pagedList)
        {
            if (request.GetType().CheckIfAnonymousType())
            {
                throw new ArgumentException("request", "Anonymous types are not supported for paging");
            }

            var url = Generate(routeName, request);
            var result = new PagingResult();

            if (pagedList != null)
            {
                var type = request.GetType();
                var clone = CloneObject(request);
                var propertyInfo = type.GetProperty(PagePropertyName);

                if (pagedList.HasNextPage)
                {
                    propertyInfo.SetValue(clone, pagedList.PageNumber + 1);
                    result.NextUrl = Generate(routeName, clone);
                }

                if (pagedList.HasPreviousPage)
                {
                    propertyInfo.SetValue(clone, pagedList.PageNumber - 1);
                    result.NextUrl = Generate(routeName, clone);
                }
            }

            return result;
        }

        public static T CloneObject<T>(T obj) where T : class
        {
            if (obj == null) return null;
            System.Reflection.MethodInfo inst = obj.GetType().GetMethod("MemberwiseClone",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (inst != null)
                return (T)inst.Invoke(obj, null);
            else
                return null;
        }
    }
}
