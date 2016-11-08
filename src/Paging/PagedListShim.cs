namespace RimDev.Supurlative.Paging
{
    /// <summary>
    /// Used to Shim PagedList into 
    /// the IPaged, which is what
    /// we want now
    /// </summary>
    internal class PagedListShim : IPaged
    {
        private readonly IPagedList shim;

        public PagedListShim(IPagedList shim)
        {
            this.shim = shim;
        }

        public int? Page => shim.PageNumber;
        public int? PageSize => shim.PageSize;
        public int? TotalItemCount => shim.TotalItemCount;
    }
}