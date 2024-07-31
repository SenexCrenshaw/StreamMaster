namespace StreamMaster.Application.Custom.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ScanForCustomRequest : IRequest<APIResponse>;

public class ScanForCustomPlayListsRequestHandler(ILogger<ScanForCustomRequest> Logger, ICryptoService cryptoService, IIconService iconService, IIntroPlayListBuilder introPlayListBuilder, ICustomPlayListBuilder CustomPlayListBuilder, IRepositoryWrapper Repository)
    : IRequestHandler<ScanForCustomRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ScanForCustomRequest command, CancellationToken cancellationToken)
    {
        List<CustomPlayList> customPlayLists = CustomPlayListBuilder.GetCustomPlayLists();
        List<string> smStreamIds = [];
        foreach (CustomPlayList customPlayList in customPlayLists)
        {
            string id = customPlayList.Name;// FileUtil.EncodeToBase64(customPlayList.Name);
            AddIcon(customPlayList);

            SMStream? currentStream = await Repository.SMStream.FirstOrDefaultAsync(s => s.Id == id, tracking: true, cancellationToken: cancellationToken);
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
                Url = "STREAMMASTER"
            };
            Repository.SMStream.Create(smStream);
            smStreamIds.Add(id);

            foreach (CustomStreamNfo nfo in customPlayList.CustomStreamNfos)
            {
                string streamId = $"{id}|{nfo.Movie.Title}";// FileUtil.EncodeToBase64($"{id},{nfo.Movie.Title}");
                SMStream? nfoStream = await Repository.SMStream.FirstOrDefaultAsync(s => s.Id == streamId, tracking: true, cancellationToken: cancellationToken);

                if (nfoStream != null)
                {
                    continue;
                }

                string? c = await cryptoService.EncodeStreamGroupIdStreamIdAsync(EPGHelper.CustomPlayListId, streamId);

                SMStream newStream = new()
                {
                    Id = streamId,
                    EPGID = EPGHelper.CustomPlayListId + "-" + nfo.Movie.Title,
                    Name = nfo.Movie.Title,
                    M3UFileName = Path.GetFileName(nfo.VideoFileName),
                    M3UFileId = EPGHelper.CustomPlayListId,
                    Group = "CustomPlayList",
                    IsCustomStream = true,
                    Url = $"http://127.0.0.1:7095/m/{c}.ts"
                };
                Repository.SMStream.Create(newStream);
            }
        }
        _ = await Repository.SaveAsync();
        _ = await Repository.SMChannel.CreateSMChannelsFromCustomStreams(smStreamIds, EPGHelper.CustomPlayListId, true);

        List<CustomPlayList> introPlayLists = introPlayListBuilder.GetIntroPlayLists();
        foreach (CustomPlayList customPlayList in introPlayLists)
        {

            foreach (CustomStreamNfo nfo in customPlayList.CustomStreamNfos)
            {
                string streamId = $"{IntroPlayListBuilder.IntroIDPrefix}{nfo.Movie.Title}";
                SMStream? nfoStream = await Repository.SMStream.FirstOrDefaultAsync(s => s.Id == streamId, tracking: true, cancellationToken: cancellationToken);

                if (nfoStream != null)
                {
                    continue;
                }

                string? c = await cryptoService.EncodeStreamGroupIdStreamIdAsync(EPGHelper.CustomPlayListId, streamId);

                SMStream newStream = new()
                {
                    Id = streamId,
                    EPGID = EPGHelper.CustomPlayListId + "-" + nfo.Movie.Title,
                    Name = nfo.Movie.Title,
                    M3UFileName = Path.GetFileName(nfo.VideoFileName),
                    M3UFileId = EPGHelper.CustomPlayListId,
                    Group = "Intros",
                    IsCustomStream = true,
                    Url = $"http://127.0.0.1:7095/m/{c}.ts"
                };
                Repository.SMStream.Create(newStream);
            }
        }
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