using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.API;

public static class APIResponseFactory
{
    public static APIResponse<T> Ok<T>(PagedResponse<T>? pagedResponse = default)
    {
        return new APIResponse<T> { Message = "OK", PagedResponse = pagedResponse };
    }

    public static DefaultAPIResponse Ok()
    {
        return new DefaultAPIResponse();
    }

    public static DefaultAPIResponse NotFound()
    {
        return new DefaultAPIResponse
        {
            IsError = true,
            ErrorMessage = "Not Found"
        };
    }

    public static APIResponse<T> Error<T>(string errorMessage, PagedResponse<T>? pagedResponse = default)
    {
        return new APIResponse<T> { ErrorMessage = errorMessage, IsError = true, PagedResponse = pagedResponse };
    }
}
