namespace RimDev.Supurlative.Paging
{
    /// <summary>
    /// Not necessary to implement,
    /// here to define the interface of
    /// PagedList
    /// https://www.nuget.org/packages/PagedList/
    /// </summary>
    public interface IPagedList
    {
        int PageNumber { get; }
        int PageSize { get; }
        int TotalItemCount { get; }
    }
}