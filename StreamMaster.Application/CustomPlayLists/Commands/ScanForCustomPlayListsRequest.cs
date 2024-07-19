using StreamMaster.Application.SMChannels.Commands;

using System.Web;

namespace StreamMaster.Application.CustomPlayLists.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ScanForCustomPlayListsRequest : IRequest<APIResponse>;

public class ScanForCustomPlayListsRequestHandler(ILogger<ScanForCustomPlayListsRequest> Logger, IIconService iconService, ISender Sender, IOptionsMonitor<Setting> intSettings, ICustomPlayListBuilder CustomPlayListBuilder, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher)
    : IRequestHandler<ScanForCustomPlayListsRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ScanForCustomPlayListsRequest command, CancellationToken cancellationToken)
    {

        List<CustomPlayList> test = CustomPlayListBuilder.GetCustomPlayLists();
        List<string> smStreamIds = [];
        foreach (CustomPlayList customPlayList in test)
        {
            string id = FileUtil.EncodeToBase64(customPlayList.Name);
            AddIcon(customPlayList);


            SMStream? currentStream = await Repository.SMStream.FirstOrDefaultAsync(s => s.Id == id, cancellationToken: cancellationToken, tracking: true);
            if (currentStream != null)
            {
                if (currentStream.Logo != customPlayList.Logo)
                {
                    currentStream.Logo = customPlayList.Logo;
                }
                SMChannel? smChannel = await Repository.SMChannel.GetQuery(true).FirstOrDefaultAsync(a => a.IsCustomStream && a.StreamID != null && a.StreamID == id, cancellationToken: cancellationToken);
                if (smChannel != null)
                {
                    if (smChannel.Logo != customPlayList.Logo)
                    {
                        smChannel.Logo = customPlayList.Logo;
                    }
                }


                continue;
            }
            Setting settings = intSettings.CurrentValue;

            string encodedName = HttpUtility.HtmlEncode(customPlayList.Name).Trim().Replace("/", "").Replace(" ", "_");
            string encodedId = id.EncodeValue128(settings.ServerKey);

            SMStream smStream = new()
            {
                Id = id,
                EPGID = EPGHelper.CustomPlayListId + "-" + customPlayList.Name,
                Name = customPlayList.Name,
                M3UFileName = customPlayList.Name,
                M3UFileId = EPGHelper.CustomPlayListId,
                Group = "CustomPlayList",
                IsCustomStream = true,
                Logo = customPlayList.Logo,
                Url = $"/api/videostreams/customstream/{encodedId}/{encodedName}"
            };
            Repository.SMStream.Create(smStream);
            smStreamIds.Add(id);
        }

        _ = await Repository.SaveAsync();

        _ = await Sender.Send(new CreateSMChannelsFromStreamsRequest(smStreamIds, M3UFileId: EPGHelper.CustomPlayListId, IsCustomPlayList: true, forced: true), cancellationToken);

        _ = await Repository.SaveAsync();

        return APIResponse.Success;
    }

    private void AddIcon(CustomPlayList customPlayList)
    {
        if (!string.IsNullOrEmpty(customPlayList.Logo))
        {
            IconFileDto iconFileDto = new()
            {
                Name = customPlayList.Name,
                Source = customPlayList.Logo,
                SMFileType = SMFileTypes.CustomPlayList
            };
            iconService.AddIcon(iconFileDto);
        }
    }

}