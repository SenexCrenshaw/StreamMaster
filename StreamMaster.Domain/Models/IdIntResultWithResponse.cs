using StreamMaster.Domain.API;

namespace StreamMaster.Domain.Models;

public class IdIntResult
{
    public int Id { get; set; }
    public required dynamic Result { get; set; }

}

public class IdIntResultWithResponse : List<IdIntResult>
{
    public APIResponse APIResponse { get; set; } = APIResponse.Ok;
}
