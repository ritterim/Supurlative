using System;
using System.Linq.Expressions;
using System.Net.Http;
using ImpromptuInterface;

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
            object paged,
            Expression<Func<T, int?>> pagePropertyExpression = null)
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

            var target = GetTypedPagedObject(paged);

            if (target != null)
            {
                var pagePropertyName =
                    ExtractPropertyNameFromExpression(pagePropertyExpression)
                    ?? DefaultPagePropertyName;
                var type = request.GetType();
                var clone = CloneObject(request);
                var propertyInfo = type.GetProperty(pagePropertyName);

                if (propertyInfo == null)
                    throw new ArgumentException(
                        $"Property \"{pagePropertyName}\" does not exist on type \"{type.FullName}\"." +
                        " Please use the pagePropertyExpression to define a valid page property."
                        , pagePropertyName);

                if (HasNextPage(target))
                {
                    propertyInfo.SetValue(clone, target.Page  + 1);
                    result.NextUrl = GenerateUrl(routeName, clone);
                }

                if (HasPreviousPage(target))
                {
                    propertyInfo.SetValue(clone, target.Page - 1);
                    result.PreviousUrl = GenerateUrl(routeName, clone);
                }
            }

            return result;
        }

        private static IPaged GetTypedPagedObject(object input)
        {
            if (input == null)
                return null;

            IPagedList pagedList;
            return IsPagedListCompatible(input, out pagedList)
                ? new PagedListShim(pagedList)
                : input.ActLike<IPaged>();
        }

        private static bool IsPagedListCompatible(object paged, out IPagedList pagedList)
        {
            var target = paged.ActLike<IPagedList>();

            if (target.Has(x => x.TotalItemCount) &&
                target.Has(x => x.PageSize) &&
                target.Has(x => x.PageNumber))
            {
                pagedList = target;
                return true;
            }

            pagedList = null;
            return false;
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

        private static string ExtractPropertyNameFromExpression<T>(Expression<Func<T, int?>> expression)
        {
            string propertyName = null;

            if (expression != null)
            {
                var unaryExpression = expression.Body as UnaryExpression;

                if (unaryExpression != null)
                {
                    var memberExpression = unaryExpression.Operand as MemberExpression;
                    if (memberExpression != null)
                    {
                        propertyName = memberExpression.Member.Name;
                    }
                }
            }

            return propertyName;
        }

        private static bool HasNextPage(IPaged target)
        {
            if (target.TotalItemCount.HasValue && target.PageSize.HasValue)
            {
                var pageCount = target?.TotalItemCount > 0
                    ? (int)Math.Ceiling((double)target.TotalItemCount / (double)target.PageSize)
                    : 0;
                return target?.Page < pageCount;
            }

            return false;
        }

        private static bool HasPreviousPage(IPaged target)
        {
            return target?.Page > 1;
        }
    }
}
