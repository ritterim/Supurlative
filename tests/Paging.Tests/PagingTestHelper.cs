using RimDev.Supurlative.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace RimDev.Supurlative.Paging.Tests
{
    public static class PagingTestHelper
    {
        public static PagingGenerator CreateAPagingGenerator(
            string baseUrl,
            string routeName,
            string routeTemplate,
            object routeDefaults = null,
            object routeConstraints = null,
            SupurlativeOptions supurlativeOptions = null
            )
        {
            HttpRequestMessage request;
            request = TestHelper.CreateAHttpRequestMessage(baseUrl, routeName, routeTemplate, routeDefaults, routeConstraints);
            return new PagingGenerator(request, supurlativeOptions);
        }
    }
}
