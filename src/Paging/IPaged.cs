namespace RimDev.Supurlative.Paging
{
    public interface IPaged
    {
        int? Page { get; }
        int? PageSize { get; }
        int? TotalItemCount { get; }
    }
}