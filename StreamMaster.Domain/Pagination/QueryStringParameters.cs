using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Pagination
{
    [TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
    public class QueryStringParameters
    {
        public QueryStringParameters() { }

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
            set => _pageSize = value;
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