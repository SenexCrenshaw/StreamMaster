using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Enums;

using StreamMaster.PlayList.Models;

namespace StreamMaster.Streams.Services;

public sealed class SwitchToNextStreamService(ILogger<SwitchToNextStreamService> logger, ICacheManager cacheManager, IStreamLimitsService streamLimitsService, IProfileService profileService, IServiceProvider _serviceProvider, IIntroPlayListBuilder introPlayListBuilder, ICustomPlayListBuilder customPlayListBuilder, IOptionsMonitor<Setting> intSettings)
    : ISwitchToNextStreamService
{
    private static string GetClientUserAgent(SMChannelDto smChannel, SMStreamDto? smStream, Setting setting)
    {
        string clientUserAgent =
            !string.IsNullOrEmpty(smStream?.ClientUserAgent) ? smStream.ClientUserAgent :
            !string.IsNullOrEmpty(smChannel.ClientUserAgent) ? smChannel.ClientUserAgent :
            setting.ClientUserAgent;

        return clientUserAgent;
    }

    public async Task<bool> SetNextStreamAsync(IStreamStatus ChannelStatus, string? OverrideSMStreamId = null)
    {
        ChannelStatus.FailoverInProgress = true;

        Setting _settings = intSettings.CurrentValue;

        if (_settings.ShowIntros != "None")
        {
            if (
                (_settings.ShowIntros == "Once" && ChannelStatus.IsFirst) ||
                (_settings.ShowIntros == "Always" && !ChannelStatus.PlayedIntro)
                )
            {
                CustomStreamNfo? intro = introPlayListBuilder.GetRandomIntro(ChannelStatus.IsFirst ? null : ChannelStatus.IntroIndex);
                CommandProfileDto introCommandProfileDto = profileService.GetCommandProfile("SMFFMPEGLocal");
                ChannelStatus.IsFirst = false;
                ChannelStatus.PlayedIntro = true;

                if (intro != null)
                {
                    string streamId = $"{IntroPlayListBuilder.IntroIDPrefix}{intro.Movie.Title}";
                    SMStreamInfo overRideSMStreamInfo = new()
                    {
                        Id = streamId,
                        Name = intro.Movie.Title,
                        Url = intro.VideoFileName,
                        ClientUserAgent = intSettings.CurrentValue.ClientUserAgent,
                        CommandProfile = introCommandProfileDto,
                        SMStreamType = SMStreamTypeEnum.Intro
                    };
                    ChannelStatus.SetSMStreamInfo(overRideSMStreamInfo);
                    logger.LogDebug("Set Next for Channel {SourceName}, switched to {Id} {Name}", ChannelStatus.SourceName, overRideSMStreamInfo.Id, overRideSMStreamInfo.Name);
                    return true;
                }
            }
        }

        using IServiceScope scope = _serviceProvider.CreateScope();

        ChannelStatus.PlayedIntro = false;

        SMStreamDto? smStream = null;
        if (!string.IsNullOrEmpty(OverrideSMStreamId))
        {
            IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
            smStream = repository.SMStream.GetSMStream(OverrideSMStreamId);
        }

        if (smStream == null || streamLimitsService.IsLimited(smStream))
        {
            if (!ChannelHasStreamsOrChannels(ChannelStatus.SMChannel))
            {
                logger.LogError("Set Next for Channel {SourceName}, {Id} {Name} starting has no streams", ChannelStatus.SourceName, ChannelStatus.SMChannel.Id, ChannelStatus.SMChannel.Name);
                ChannelStatus.SetSMStreamInfo(null);
                return false;
            }

            List<SMStreamDto> smStreams = [.. ChannelStatus.SMChannel.SMStreamDtos.OrderBy(a => a.Rank)];

            bool isChannelLimited = true;

            while (isChannelLimited)
            {
                if (ChannelStatus.SMChannel.CurrentRank + 1 >= ChannelStatus.SMChannel.SMStreamDtos.Count)
                {
                    logger.LogInformation("Set Next for Channel {SourceName}, {Id} {Name} at end of stream list", ChannelStatus.SourceName, ChannelStatus.SMChannel.Id, ChannelStatus.SMChannel.Name);
                    break;
                }

                ChannelStatus.SMChannel.CurrentRank = (ChannelStatus.SMChannel.CurrentRank + 1) % smStreams.Count;
                smStream = smStreams[ChannelStatus.SMChannel.CurrentRank];
                isChannelLimited = streamLimitsService.IsLimited(smStream);
                if (!isChannelLimited)
                {
                    break;
                }
            }
        }

        if (smStream == null)
        {
            logger.LogInformation("Set Next for Channel {SourceName}, {Id} {Name}, max Streams reached, trying next in list", ChannelStatus.SourceName, ChannelStatus.SMChannel.Id, ChannelStatus.SMChannel.Name);
            return false;
        }
        bool isLimited = streamLimitsService.IsLimited(smStream);
        if (isLimited)
        {
            if (_settings.ShowMessageVideos && cacheManager.MessageNoStreamsLeft != null)
            {
                ChannelStatus.SetSMStreamInfo(cacheManager.MessageNoStreamsLeft);
                logger.LogDebug("No more streams found, {SourceName} {Id} {Name} , sending message", ChannelStatus.SourceName, cacheManager.MessageNoStreamsLeft.Id, cacheManager.MessageNoStreamsLeft.Name);
                return true;
            }
            logger.LogInformation("Set Next for Channel {SourceName}, {Id} {Name}, max Streams reached", ChannelStatus.SourceName, ChannelStatus.SMChannel.Id, ChannelStatus.SMChannel.Name);
            return false;
        }

        IStreamGroupService streamGroupService = scope.ServiceProvider.GetRequiredService<IStreamGroupService>();

        string clientUserAgent = GetClientUserAgent(ChannelStatus.SMChannel, smStream, intSettings.CurrentValue);

        if (smStream.SMStreamType is SMStreamTypeEnum.CustomPlayList)
        {
            CustomPlayList? customPlayList = customPlayListBuilder.GetCustomPlayList(smStream.Name);

            if (customPlayList == null)
            {
                return false;
            }

            ChannelStatus.CustomPlayList = customPlayList;

            (CustomStreamNfo StreamNfo, int SecondsIn) = customPlayListBuilder.GetCurrentVideoAndElapsedSeconds(ChannelStatus.CustomPlayList.Name);

            CommandProfileDto customPlayListProfileDto = profileService.GetCommandProfile("SMFFMPEGLocal");

            SMStreamInfo customSMStreamInfo = new()
            {
                Id = StreamNfo.Movie.Title,
                Name = StreamNfo.Movie.Title,
                Url = StreamNfo.VideoFileName,
                SMStreamType = SMStreamTypeEnum.CustomPlayList,
                SecondsIn = SecondsIn,
                ClientUserAgent = clientUserAgent,
                CommandProfile = customPlayListProfileDto
            };

            ChannelStatus.SetSMStreamInfo(customSMStreamInfo);
            logger.LogDebug("Set Next for Channel {SourceName}, switched to {Id} {Name} starting at {SecondsIn} seconds in", ChannelStatus.SourceName, customSMStreamInfo.Id, customSMStreamInfo.Name, SecondsIn);
            return await Task.FromResult(true);
        }

        if (smStream.SMStreamType is SMStreamTypeEnum.Intro)
        {
            CustomPlayList? customPlayList = introPlayListBuilder.GetIntroPlayList(smStream.Name);

            if (customPlayList == null)
            {
                return false;
            }

            ChannelStatus.CustomPlayList = customPlayList;

            CommandProfileDto customPlayListProfileDto = profileService.GetCommandProfile("SMFFMPEGLocal");

            SMStreamInfo customSMStreamInfo = new()
            {
                Id = customPlayList.Name,
                Name = customPlayList.Name,
                Url = customPlayList.CustomStreamNfos[0].VideoFileName,
                SMStreamType = SMStreamTypeEnum.CustomPlayList,
                SecondsIn = 0,
                ClientUserAgent = clientUserAgent,
                CommandProfile = customPlayListProfileDto
            };

            ChannelStatus.SetSMStreamInfo(customSMStreamInfo);
            logger.LogDebug("Set Next for Channel {SourceName}, switched to {Id} {Name}", ChannelStatus.SourceName, customSMStreamInfo.Id, customSMStreamInfo.Name);
            return await Task.FromResult(true);
        }

        CommandProfileDto commandProfileDto = string.IsNullOrEmpty(smStream.CommandProfileName)
            ? await streamGroupService.GetProfileFromSGIdsCommandProfileNameAsync(null, ChannelStatus.StreamGroupProfileId, ChannelStatus.SMChannel.CommandProfileName)
            : await streamGroupService.GetProfileFromSGIdsCommandProfileNameAsync(null, ChannelStatus.StreamGroupProfileId, smStream.CommandProfileName);

        SMStreamInfo smStreamInfo = new()
        {
            Id = smStream.Id,
            Name = smStream.Name,
            Url = smStream.Url,
            SMStreamType = smStream.SMStreamType,
            ClientUserAgent = clientUserAgent,
            CommandProfile = commandProfileDto
        };

        ChannelStatus.SetSMStreamInfo(smStreamInfo);
        logger.LogDebug("Set Next for Channel {SourceName}, switched to {Id} {Name}", ChannelStatus.SourceName, smStreamInfo.Id, smStreamInfo.Name);

        return true;
    }

    private static bool ChannelHasStreamsOrChannels(SMChannelDto smChannel)
    {
        return smChannel.SMChannelType == SMChannelTypeEnum.MultiView
            ? smChannel.SMChannelDtos.Count > 0
            : smChannel.SMStreamDtos.Count > 0 && !string.IsNullOrEmpty(smChannel.SMStreamDtos[0].Url);
    }
}