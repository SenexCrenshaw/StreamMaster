using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.API;

public static class APIResponseFactory
{
    public static APIResponse<T> OkWithData<T>(PagedResponse<T>? pagedResponse = default)
    {
        return new APIResponse<T> { Message = "OK", PagedResponse = pagedResponse };
    }

    public static DefaultAPIResponse OkWithMessage(string message)
    {
        DefaultAPIResponse ok = Ok;
        ok.Message = message;
        return ok;
    }
    public static DefaultAPIResponse ErrorWithMessage(string message)
    {
        DefaultAPIResponse ok = new()
        {
            IsError = true,
            ErrorMessage = message
        };
        return ok;
    }

    public static DefaultAPIResponse Ok => new();

    public static DefaultAPIResponse NotFound =>
     new()
     {
         IsError = true,
         ErrorMessage = "Not Found"
     };

    public static APIResponse<T> Error<T>(string errorMessage, PagedResponse<T>? pagedResponse = default)
    {
        return new APIResponse<T> { ErrorMessage = errorMessage, IsError = true, PagedResponse = pagedResponse };
    }
}
