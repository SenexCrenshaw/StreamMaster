namespace StreamMaster.Application.Custom.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record ScanForCustomRequest : IRequest<APIResponse>;

public class ScanForCustomPlayListsRequestHandler(IOptionsMonitor<CommandProfileDict> optionsOutputProfiles, IOptionsMonitor<Setting> _settings, ICacheManager cacheManager, IIntroPlayListBuilder introPlayListBuilder, ICustomPlayListBuilder CustomPlayListBuilder, IRepositoryWrapper Repository)
    : IRequestHandler<ScanForCustomRequest, APIResponse>
{
    public async Task<APIResponse> Handle(ScanForCustomRequest command, CancellationToken cancellationToken)
    {
        List<CustomPlayList> customPlayLists = CustomPlayListBuilder.GetCustomPlayLists();
        List<string> smStreamIdsToChannels = [];

        foreach (CustomPlayList customPlayList in customPlayLists)
        {
            string id = customPlayList.Name;

            SMStream? currentStream = await Repository.SMStream.FirstOrDefaultAsync(s => s.Id == id, tracking: true, cancellationToken: cancellationToken);
            if (currentStream != null)
            {
                if (currentStream.Logo != customPlayList.Logo)
                {
                    currentStream.Logo = customPlayList.Logo;
                }

                continue;
            }

            //string logo = logoService.GetLogoUrl2(customPlayList.Logo, SMFileTypes.CustomPlayListLogo);
            string logo = customPlayList.Logo.Remove(0, BuildInfo.CustomPlayListFolder.Length);

            SMStream smStream = new()
            {
                Id = id,
                EPGID = EPGHelper.CustomPlayListId + "-" + customPlayList.Name,
                Name = customPlayList.Name,
                M3UFileName = customPlayList.Name,
                M3UFileId = EPGHelper.CustomPlayListId,
                Group = "CustomPlayList",
                SMStreamType = SMStreamTypeEnum.CustomPlayList,
                Logo = logo,
                Url = "STREAMMASTER",
                IsSystem = true,
            };
            Repository.SMStream.Create(smStream);
            smStreamIdsToChannels.Add(smStream.Id);

            foreach (CustomStreamNfo nfo in customPlayList.CustomStreamNfos)
            {
                string streamId = $"{id}|{nfo.Movie.Title}";
                SMStream? nfoStream = await Repository.SMStream.FirstOrDefaultAsync(s => s.Id == streamId, tracking: true, cancellationToken: cancellationToken);

                if (nfoStream != null)
                {
                    continue;
                }

                //string? c = await streamGroupService.EncodeStreamGroupIdStreamIdAsync(EPGHelper.CustomPlayListId, streamId);
                string logo2 = nfo.Movie.Thumb?.Text ?? nfo.Movie.Fanart?.Thumb?.Text ?? customPlayList.Logo;

                if (logo.StartsWith(BuildInfo.CustomPlayListFolder))
                {
                    logo = logo.Remove(0, BuildInfo.CustomPlayListFolder.Length);
                }

                SMStream newStream = new()
                {
                    Id = streamId,
                    EPGID = EPGHelper.CustomPlayListId + "-" + nfo.Movie.Title,
                    Name = nfo.Movie.Title,
                    M3UFileName = Path.GetFileName(nfo.VideoFileName),
                    M3UFileId = EPGHelper.CustomPlayListId,
                    Group = "CustomPlayList",
                    SMStreamType = SMStreamTypeEnum.Custom,
                    Url = nfo.VideoFileName,
                    Logo = logo,
                    IsSystem = true,
                };

                Repository.SMStream.Create(newStream);
            }
        }
        _ = await Repository.SaveAsync();

        _ = await Repository.SMChannel.CreateSMChannelsFromStreams(smStreamIdsToChannels, null);

        List<CustomPlayList> introPlayLists = introPlayListBuilder.GetIntroPlayLists();
        foreach (CustomPlayList customPlayList in introPlayLists)
        {
            foreach (CustomStreamNfo nfo in customPlayList.CustomStreamNfos)
            {
                string streamId = $"{IntroPlayListBuilder.IntroIDPrefix}{nfo.Movie.Title}";

                SMStream? nfoStream = await Repository.SMStream.FirstOrDefaultAsync(s => s.Id == streamId, tracking: true, cancellationToken: cancellationToken);

                if (nfoStream != null)
                {
                    if (nfoStream.Logo != customPlayList.Logo)
                    {
                        nfoStream.Logo = customPlayList.Logo;
                    }
                    continue;
                }

                SMStream newStream = new()
                {
                    Id = streamId,
                    EPGID = EPGHelper.IntroPlayListId + "-" + nfo.Movie.Title,
                    Name = nfo.Movie.Title,
                    M3UFileName = Path.GetFileName(nfo.VideoFileName),
                    M3UFileId = EPGHelper.IntroPlayListId,
                    Group = "Intros",
                    SMStreamType = SMStreamTypeEnum.Intro,
                    Url = nfo.VideoFileName,
                    IsSystem = true
                };
                Repository.SMStream.Create(newStream);
            }
        }
        _ = await Repository.SaveAsync();

        if (File.Exists(BuildInfo.MessageNoStreamsLeft))
        {
            SMStream? stream = await Repository.SMStream.FirstOrDefaultAsync(a => a.Id == "MessageNoStreamsLeft", cancellationToken: cancellationToken);
            if (stream == null)
            {
                stream = new()
                {
                    Id = "MessageNoStreamsLeft",
                    EPGID = EPGHelper.MessageId + "-MessageNoStreamsLeft",
                    Name = "No Streams Left",
                    M3UFileName = Path.GetFileName(BuildInfo.MessageNoStreamsLeft),
                    M3UFileId = EPGHelper.IntroPlayListId,
                    Group = "SystemMessages",
                    SMStreamType = SMStreamTypeEnum.Message,
                    Url = BuildInfo.MessageNoStreamsLeft,
                    IsSystem = true
                };
                Repository.SMStream.Create(stream);
                await Repository.SaveAsync();
            }
            if (stream != null)
            {
                CommandProfileDto introCommandProfileDto = optionsOutputProfiles.CurrentValue.GetProfileDto("SMFFMPEG");
                SMStreamInfo smStreamInfo = new()
                {
                    Id = stream.Id,
                    Name = stream.Name,
                    Url = stream.Url,
                    ClientUserAgent = _settings.CurrentValue.ClientUserAgent,
                    CommandProfile = introCommandProfileDto,
                    SMStreamType = SMStreamTypeEnum.Message
                };
                cacheManager.MessageNoStreamsLeft = smStreamInfo;
            }
        }

        return APIResponse.Success;
    }
}