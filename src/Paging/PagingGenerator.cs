using PagedList;
using System;
using System.Linq.Expressions;
using System.Net.Http;

namespace RimDev.Supurlative.Paging
{
    public class PagingGenerator : Generator
    {
        public PagingGenerator(
            HttpRequestMessage requestMessage,
            SupurlativeOptions options = null,
            string pagePropertyName = "page")
            : base(requestMessage, options)
        {
            DefaultPagePropertyName = pagePropertyName;
        }

        public string DefaultPagePropertyName { get; protected set; }

        public PagingResult Generate<T>(
            string routeName,
            T request,
            IPagedList pagedList,
            Expression<Func<T, int>> pagePropertyExpression = null)
            where T : class
        {
            if (request.GetType().CheckIfAnonymousType())
            {
                throw new ArgumentException("request", "Anonymous types are not supported for paging");
            }

            var generated = Generate(routeName, request);
            var result = new PagingResult
            {
                Url = generated.Url,
                Template = generated.Template
            };

            if (pagedList != null)
            {
                var pagePropertyName =
                    ExtractPropertyNameFromExpression(pagePropertyExpression)
                    ?? DefaultPagePropertyName;
                var type = request.GetType();
                var clone = CloneObject(request);
                var propertyInfo = type.GetProperty(pagePropertyName);

                if (propertyInfo == null)
                    throw new ArgumentException(
                            string.Format("Property \"{0}\" does not exist on type \"{1}\"." +
                            " Please use the pagePropertyExpression to define a valid page property.", pagePropertyName, type.FullName)
                        , pagePropertyName);

                if (pagedList.HasNextPage)
                {
                    propertyInfo.SetValue(clone, pagedList.PageNumber + 1);
                    result.NextUrl = GenerateUrl(routeName, clone);
                }

                if (pagedList.HasPreviousPage)
                {
                    propertyInfo.SetValue(clone, pagedList.PageNumber - 1);
                    result.PreviousUrl = GenerateUrl(routeName, clone);
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

        private string ExtractPropertyNameFromExpression<T>(Expression<Func<T, int>> expression)
        {
            string propertyName = null;

            if (expression != null)
            {
                var memberExpression = expression.Body as MemberExpression;

                if (memberExpression != null)
                {
                    propertyName = memberExpression.Member.Name;
                }
            }

            return propertyName;
        }
    }
}
