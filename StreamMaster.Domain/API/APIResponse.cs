using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.API
{
    public class APIResponse<T>
    {
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public bool? IsError { get; set; }
        public PagedResponse<T>? PagedResponse { get; set; }
    }
}
