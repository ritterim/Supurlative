using PagedList;
using RimDev.Supurlative.Tests;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Xunit;

namespace RimDev.Supurlative.Paging.Tests
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
        private HttpRequestMessage HttpRequest { get; set; }

        private PagingGenerator InitializeGenerator(SupurlativeOptions options = null)
        {
            HttpRequest = WebApiHelper.GetRequest();
            var configuration = HttpRequest.GetConfiguration();

            configuration.Routes.MapHttpRoute("page.query", "paging");
            configuration.Routes.MapHttpRoute("page.path", "paging/{page}");
            configuration.Routes.MapHttpRoute("newPage.path", "paging/{currentPageNumber}");

            return new PagingGenerator(HttpRequest);
        }

        [Fact]
        public void Can_generate_paged_result_with_query_string()
        {
           var result =  Generator.Generate("page.query", new Request { page = 1 }, PagedList);

            Assert.True(result.HasNext);
            Assert.False(result.HasPrevious);

            Assert.Equal("http://localhost:8000/paging?page=2&currentpagenumber=0", result.NextUrl);
        }

        [Fact]
        public void Can_generate_paged_result_with_query_string_and_page_expression()
        {
            var result = Generator.Generate("page.query", new Request { page = 1 }, PagedList, x => x.currentPageNumber);

            Assert.True(result.HasNext);
            Assert.False(result.HasPrevious);

            Assert.Equal("http://localhost:8000/paging?page=1&currentpagenumber=2", result.NextUrl);
        }

        [Fact]
        public void Can_generate_paged_result_with_path()
        {
            var result = Generator.Generate("page.path", new Request { page = 1 }, PagedList);

            Assert.True(result.HasNext);
            Assert.Equal("http://localhost:8000/paging/2?currentpagenumber=0", result.NextUrl);
        }

        [Fact]
        public void Can_generate_paged_result_with_path_and_page_expression()
        {
            var result = Generator.Generate("newPage.path", new Request { page = 1 }, PagedList, x => x.currentPageNumber);

            Assert.True(result.HasNext);
            Assert.Equal("http://localhost:8000/paging/2?page=1", result.NextUrl);
        }

        [Fact]
        public void Can_generate_paged_result_with_previous_url()
        {
            var result = Generator.Generate("newPage.path", new Request(), PagedList.ToPagedList(2, 10));

            Assert.True(result.HasPrevious);
            Assert.Equal("http://localhost:8000/paging/0?page=1", result.PreviousUrl);
        }

        [Fact]
        public void Throws_meaningful_exception_when_page_doesnt_exist()
        {
            var exception =
            Assert.Throws<ArgumentException>(
                () => new PagingGenerator(HttpRequest, Generator.Options, "Poop").Generate("newPage.path", new Request(), PagedList)
            );
        }

        public class Request
        {
            public int page { get; set; }

            public int currentPageNumber { get; set; }
        }
    }
}
