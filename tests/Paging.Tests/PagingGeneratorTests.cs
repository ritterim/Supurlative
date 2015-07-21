using RimDev.Supurlative;
using RimDev.Supurlative.Paging;
using RimDev.Supurlative.Tests;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Xunit;

namespace PagedList.Tests
{
    public class PagingGneratorTests
    {
        public PagingGneratorTests()
        {
            Generator = InitializeGenerator();

            PagedList = Enumerable
                .Range(1, 100)
                .ToPagedList(1, 10);

        }

        private readonly IPagedList<int> PagedList;
        private readonly PagingGenerator Generator;

        private static PagingGenerator InitializeGenerator(SupurlativeOptions options = null)
        {
            var request = WebApiHelper.GetRequest();
            var configuration = request.GetConfiguration();

            configuration.Routes.MapHttpRoute("page.query", "paging");
            configuration.Routes.MapHttpRoute("page.path", "paging/{page}");

            return new PagingGenerator(request);
        }

        [Fact]
        public void Can_generate_paged_result_with_query_string()
        {
           var result =  Generator.Generate("page.query", new Request { page = 1 }, PagedList);

            Assert.True(result.HasNext);
            Assert.False(result.HasPrevious);

            Assert.Equal("http://localhost:8000/paging?page=2", result.NextUrl);
        }

        [Fact]
        public void Can_generate_paged_result_with_path()
        {
            var result = Generator.Generate("page.path", new Request { page = 1 }, PagedList);

            Assert.True(result.HasNext);
            Assert.Equal("http://localhost:8000/paging/2", result.NextUrl);
        }

        public class Request
        {
            public int page { get; set; }
        }
    }
}
