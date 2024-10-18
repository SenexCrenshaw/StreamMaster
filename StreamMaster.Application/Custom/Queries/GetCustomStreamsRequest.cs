using Microsoft.AspNetCore.Http;
namespace StreamMaster.Application.Custom.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetCustomPlayListsRequest() : IRequest<DataResponse<List<CustomPlayList>>>;

public class GetCustomPlayListsRequestHandler(IHttpContextAccessor httpContextAccessor, ICustomPlayListBuilder customPlayListBuilder)
    : IRequestHandler<GetCustomPlayListsRequest, DataResponse<List<CustomPlayList>>>
{
    public async Task<DataResponse<List<CustomPlayList>>> Handle(GetCustomPlayListsRequest request, CancellationToken cancellationToken = default)
    {
        if (httpContextAccessor.HttpContext?.Request?.Path.Value == null)
        {
            return DataResponse<List<CustomPlayList>>.NotFound;
        }


        List<CustomPlayList> customPlayLists = customPlayListBuilder.GetCustomPlayLists();

        return await Task.FromResult(DataResponse<List<CustomPlayList>>.Success(customPlayLists));
    }
}