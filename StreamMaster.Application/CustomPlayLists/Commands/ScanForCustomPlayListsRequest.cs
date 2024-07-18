using System.Web;

namespace StreamMaster.Application.CustomPlayLists.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ScanForCustomPlayListsRequest : IRequest<APIResponse>;

public class ScanForCustomPlayListsRequestHandler(ILogger<ScanForCustomPlayListsRequest> Logger, IOptionsMonitor<Setting> intSettings, ICustomPlayListBuilder CustomPlayListBuilder, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher)
    : IRequestHandler<ScanForCustomPlayListsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ScanForCustomPlayListsRequest command, CancellationToken cancellationToken)
    {

        List<CustomPlayList> test = CustomPlayListBuilder.GetCustomPlayLists();
        foreach (CustomPlayList customPlayList in test)
        {
            string id = FileUtil.EncodeToBase64(customPlayList.Name);
            if (Repository.SMStream.Any(s => s.Id == id))
            {
                continue;
            }
            var settings = intSettings.CurrentValue;

            string encodedName = HttpUtility.HtmlEncode(customPlayList.Name).Trim().Replace("/", "").Replace(" ", "_");
            string encodedId = id.EncodeValue128(settings.ServerKey);

            var smStream = new SMStream
            {
                Id = id,
                Name = customPlayList.Name,
                M3UFileName = "",
                M3UFileId = EPGHelper.CustomPlayListId,
                Group = "Dummy",
                IsCustomStream = true,
                Logo = customPlayList.Logo,
                Url = $"/api/videostreams/customstream/{encodedId}/{encodedName}"
            };
            Repository.SMStream.Create(smStream);

        }

        await Repository.SaveAsync();

        return APIResponse.Success;
    }

}