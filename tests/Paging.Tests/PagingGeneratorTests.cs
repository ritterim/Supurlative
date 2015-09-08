using PagedList;
using RimDev.Supurlative.Tests;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Xunit;

namespace RimDev.Supurlative.Paging.Tests
{
    public class PagingGeneratorTests
    {
        const string _baseUrl = "http://localhost:8000/";

        public PagingGeneratorTests()
        {
            PagedList = Enumerable
                .Range(1, 100)
                .ToPagedList(1, 10);

        }

        private readonly IPagedList<int> PagedList;
        [Fact]
        public void Can_generate_paged_result_with_query_string()
        {
            string expectedUrl = _baseUrl + "paging?page=2&currentpagenumber=0";
            const string routeName = "page.query";
            const string routeTemplate = "paging";

            PagingResult result = PagingTestHelper.CreateAPagingGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new Request { page = 1 }, PagedList);

            Assert.True(result.HasNext);
            Assert.False(result.HasPrevious);
            Assert.Equal(expectedUrl, result.NextUrl);
        }

        [Fact]
        public void Can_generate_paged_result_with_query_string_and_page_expression()
        {
            string expectedUrl = _baseUrl + "paging?page=1&currentpagenumber=2";
            const string routeName = "page.query";
            const string routeTemplate = "paging";

            PagingResult result = PagingTestHelper.CreateAPagingGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new Request { page = 1 }, PagedList, x => x.currentPageNumber);

            Assert.True(result.HasNext);
            Assert.False(result.HasPrevious);
            Assert.Equal(expectedUrl, result.NextUrl);
        }

        [Fact]
        public void Can_generate_paged_result_with_path()
        {
            string expectedUrl = _baseUrl + "paging/2?currentpagenumber=0";
            const string routeName = "page.path";
            const string routeTemplate = "paging/{page}";

            PagingResult result = PagingTestHelper.CreateAPagingGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new Request { page = 1 }, PagedList);

            Assert.True(result.HasNext);
            Assert.Equal(expectedUrl, result.NextUrl);
        }

        [Fact]
        public void Can_generate_paged_result_with_path_and_page_expression()
        {
            string expectedUrl = _baseUrl + "paging/2?page=1";
            const string routeName = "newPage.path";
            const string routeTemplate = "paging/{currentPageNumber}";

            PagingResult result = PagingTestHelper.CreateAPagingGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new Request { page = 1 }, PagedList, x => x.currentPageNumber);

            Assert.True(result.HasNext);
            Assert.Equal(expectedUrl, result.NextUrl);
        }

        [Fact]
        public void Can_generate_paged_result_with_previous_url()
        {
            string expectedUrl = _baseUrl + "paging/0?page=1";
            const string routeName = "newPage.path";
            const string routeTemplate = "paging/{currentPageNumber}";

            PagingResult result = PagingTestHelper.CreateAPagingGenerator(_baseUrl, routeName, routeTemplate)
                .Generate(routeName, new Request { page = 1 }, PagedList.ToPagedList(2, 10));

            Assert.True(result.HasPrevious);
            Assert.Equal(expectedUrl, result.PreviousUrl);
        }

        [Fact]
        public void Throws_meaningful_exception_when_page_doesnt_exist()
        {
            const string routeName = "newPage.path";
            const string routeTemplate = "paging/{currentPageNumber}";

            HttpRequestMessage request;
            request = TestHelper.CreateAHttpRequestMessage(_baseUrl, routeName, routeTemplate);

            var exception =
            Assert.Throws<ArgumentException>(
                () => new PagingGenerator(request, SupurlativeOptions.Defaults, "DoesNotExist").Generate(routeName, new Request(), PagedList)
            );
        }

        public class Request
        {
            public int page { get; set; }
            public int currentPageNumber { get; set; }
        }
    }
}
