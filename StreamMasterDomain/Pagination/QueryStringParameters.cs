namespace StreamMasterDomain.Pagination
{
    public abstract class QueryStringParameters
    {
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 25;
        private string orderBy;

        public int PageSize
        {
            get
            {
                if (_pageSize == 0)
                {
                    _pageSize = 25;
                }
                return _pageSize;
            }
            set => _pageSize = value;// > maxPageSize ? maxPageSize : value;
        }

        public string OrderBy
        {
            get => orderBy; set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    orderBy = value;
                }
            }
        }
        public string? JSONArgumentString { get; set; }
        public string? JSONFiltersString { get; set; }
    }
}