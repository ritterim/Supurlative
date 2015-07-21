namespace RimDev.Supurlative.Paging
{
    public class PagingResult : Result
    {
        public string NextUrl { get; set; }
        public string PreviousUrl { get; set; }

        public bool HasNext
        {
            get
            {
                return !string.IsNullOrEmpty(NextUrl);
            }
        }

        public bool HasPrevious
        {
            get
            {
                return !string.IsNullOrEmpty(PreviousUrl);
            }
        }
    }
}
