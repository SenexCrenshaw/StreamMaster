namespace StreamMaster.Application.CustomPlayLists.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetCustomPlayListRequest(string? SMStreamId) : IRequest<DataResponse<CustomPlayList?>>;

public class GetCustomStreamRequestHandler(ICustomPlayListBuilder customPlayListBuilder)
    : IRequestHandler<GetCustomPlayListRequest, DataResponse<CustomPlayList?>>
{
    public async Task<DataResponse<CustomPlayList?>> Handle(GetCustomPlayListRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.SMStreamId))
        {
            return DataResponse<CustomPlayList?>.ErrorWithMessage("SMStreamId is required");
        }

        CustomPlayList? customPlayList = customPlayListBuilder.GetCustomPlayList(request.SMStreamId);

        return DataResponse<CustomPlayList?>.Success(customPlayList);
    }
}