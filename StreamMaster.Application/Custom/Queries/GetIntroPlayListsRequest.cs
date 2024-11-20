using Microsoft.AspNetCore.Http;
namespace StreamMaster.Application.Custom.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetIntroPlayListsRequest() : IRequest<DataResponse<List<CustomPlayList>>>;

public class GetIntroPlayListsRequestHandler(IHttpContextAccessor httpContextAccessor, IIntroPlayListBuilder introPlayListBuilder)
    : IRequestHandler<GetIntroPlayListsRequest, DataResponse<List<CustomPlayList>>>
{
    public async Task<DataResponse<List<CustomPlayList>>> Handle(GetIntroPlayListsRequest request, CancellationToken cancellationToken = default)
    {
        if (httpContextAccessor.HttpContext?.Request?.Path.Value == null)
        {
            return DataResponse<List<CustomPlayList>>.NotFound;
        }

        List<CustomPlayList> customPlayLists = introPlayListBuilder.GetIntroPlayLists();

        return await Task.FromResult(DataResponse<List<CustomPlayList>>.Success(customPlayLists));
    }
}