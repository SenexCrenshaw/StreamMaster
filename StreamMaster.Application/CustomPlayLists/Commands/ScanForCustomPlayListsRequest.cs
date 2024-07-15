using StreamMaster.PlayList;

namespace StreamMaster.Application.CustomPlayLists.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ScanForCustomPlayListsRequest : IRequest<APIResponse>;

public class ScanForCustomPlayListsRequestHandler(ILogger<ScanForCustomPlayListsRequest> Logger, ICustomPlayListBuilder CustomPlayListBuilder, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher)
    : IRequestHandler<ScanForCustomPlayListsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ScanForCustomPlayListsRequest command, CancellationToken cancellationToken)
    {
        List<CustomPlayList> test = CustomPlayListBuilder.GetNFOs();
        foreach (CustomPlayList customPlayList in test)
        {

        }

        return APIResponse.Success;
    }

}