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
            string id = FileUtil.EncodeUrlToBase64(customPlayList.Name);
            if (Repository.SMStream.Any(s => s.Id == id))
            {
                continue;
            }
            var smStrem = new SMStream
            {
                Id = id,
                Name = customPlayList.Name,
                M3UFileName = "",
                M3UFileId = EPGHelper.CustomPlayListId,
                Group = "Dummy",
                IsCustomStream = true,
                Logo = customPlayList.Logo
            };
            Repository.SMStream.Create(smStrem);

        }

        return APIResponse.Success;
    }

}