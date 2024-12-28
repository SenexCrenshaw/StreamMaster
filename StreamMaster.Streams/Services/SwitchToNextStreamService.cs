using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Enums;
using StreamMaster.PlayList.Models;

namespace StreamMaster.Streams.Services;

/// <summary>
/// Service to handle switching to the next stream for a given channel status.
/// </summary>
public sealed class SwitchToNextStreamService(
    ILogger<SwitchToNextStreamService> logger,
    ICacheManager cacheManager,
    IStreamLimitsService streamLimitsService,
    IProfileService profileService,
    IServiceProvider serviceProvider,
    IIntroPlayListBuilder introPlayListBuilder,
    ICustomPlayListBuilder customPlayListBuilder,
        IStreamConnectionService streamConnectionService,
    IOptionsMonitor<Setting> settingsMonitor) : ISwitchToNextStreamService
{
    /// <inheritdoc/>
    public async Task<bool> SetNextStreamAsync(IStreamStatus channelStatus, string? overrideSMStreamId = null)
    {
        channelStatus.FailoverInProgress = true;
        Setting settings = settingsMonitor.CurrentValue;

        if (HandleIntroLogic(channelStatus, settings))
        {
            return true;
        }

        using IServiceScope scope = serviceProvider.CreateScope();
        channelStatus.PlayedIntro = false;

        SMStreamDto? smStream = await ResolveSMStreamAsync(scope, channelStatus, overrideSMStreamId).ConfigureAwait(false);
        if (smStream == null)
        {
            HandleStreamNotFound(channelStatus);
            return false;
        }

        Domain.Metrics.StreamConnectionMetricManager? test = streamConnectionService.Get(smStream.Id);
        if (test is not null)
        {
            int currentRetry = test.GetRetryCount();

            if (currentRetry >= settings.StreamRetryLimit && test.MetricData.LastRetryTime is not null && test.MetricData.LastRetryTime.Value.AddHours(settings.StreamRetryHours) > DateTime.UtcNow)
            {
                logger.LogInformation("Stream {Name} retry limit ({currentRetry}) reached.", smStream.Name, currentRetry);
                return false;
            }
        }

        return streamLimitsService.IsLimited(smStream)
            ? HandleStreamLimits(channelStatus, settings)
            : smStream.SMStreamType switch
            {
                SMStreamTypeEnum.CustomPlayList => HandleCustomPlayListStream(channelStatus, smStream, settings),
                SMStreamTypeEnum.Intro => HandleIntroStream(channelStatus, smStream, settings),
                _ => await HandleStandardStreamAsync(scope, channelStatus, smStream, settings).ConfigureAwait(false)
            };
    }

    private static string GetClientUserAgent(SMChannelDto smChannel, SMStreamDto? smStream, Setting settings)
    {
        return !string.IsNullOrEmpty(smStream?.ClientUserAgent) ? smStream.ClientUserAgent
            : !string.IsNullOrEmpty(smChannel.ClientUserAgent) ? smChannel.ClientUserAgent
            : settings.ClientUserAgent;
    }

    private bool HandleIntroLogic(IStreamStatus channelStatus, Setting settings)
    {
        if (settings.ShowIntros != "None" &&
            ((settings.ShowIntros == "Once" && channelStatus.IsFirst) ||
             (settings.ShowIntros == "Always" && !channelStatus.PlayedIntro)))
        {
            CustomStreamNfo? intro = introPlayListBuilder.GetRandomIntro(channelStatus.IsFirst ? null : channelStatus.IntroIndex);
            if (intro != null)
            {
                SMStreamInfo introStreamInfo = CreateIntroStreamInfo(intro, settings);
                channelStatus.SetSMStreamInfo(introStreamInfo);

                channelStatus.IsFirst = false;
                channelStatus.PlayedIntro = true;

                logger.LogDebug("Set Next for Channel {SourceName}, switched to Intro {Id} {Name}",
                    channelStatus.SourceName, introStreamInfo.Id, introStreamInfo.Name);

                return true;
            }
        }

        return false;
    }

    private bool HandleIntroStream(IStreamStatus channelStatus, SMStreamDto smStream, Setting settings)
    {
        CustomPlayList? introPlayList = introPlayListBuilder.GetIntroPlayList(smStream.Name);
        if (introPlayList == null)
        {
            return false;
        }

        CommandProfileDto customPlayListProfile = profileService.GetCommandProfile("SMFFMPEGLocal");

        SMStreamInfo streamInfo = new()
        {
            Id = introPlayList.Name,
            Name = introPlayList.Name,
            Url = introPlayList.CustomStreamNfos[0].VideoFileName,
            SMStreamType = SMStreamTypeEnum.CustomPlayList,
            ClientUserAgent = GetClientUserAgent(channelStatus.SMChannel, smStream, settings),
            CommandProfile = customPlayListProfile
        };

        channelStatus.SetSMStreamInfo(streamInfo);

        logger.LogDebug("Set Next for Channel {SourceName}, switched to Intro {Id} {Name}",
            channelStatus.SourceName, streamInfo.Id, streamInfo.Name);

        return true;
    }

    private SMStreamInfo CreateIntroStreamInfo(CustomStreamNfo intro, Setting settings)
    {
        CommandProfileDto introCommandProfile = profileService.GetCommandProfile("SMFFMPEGLocal");
        return new SMStreamInfo
        {
            Id = $"{IntroPlayListBuilder.IntroIDPrefix}{intro.Movie.Title}",
            Name = intro.Movie.Title,
            Url = intro.VideoFileName,
            ClientUserAgent = settings.ClientUserAgent,
            CommandProfile = introCommandProfile,
            SMStreamType = SMStreamTypeEnum.Intro
        };
    }

    private async Task<SMStreamDto?> ResolveSMStreamAsync(IServiceScope scope, IStreamStatus channelStatus, string? overrideSMStreamId)
    {
        if (!string.IsNullOrEmpty(overrideSMStreamId))
        {
            IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
            return await repository.SMStream.GetSMStreamAsync(overrideSMStreamId).ConfigureAwait(false);
        }

        List<SMStreamDto> smStreams = [.. channelStatus.SMChannel.SMStreamDtos.OrderBy(a => a.Rank)];
        for (int i = 0; i < smStreams.Count; i++)
        {
            channelStatus.SMChannel.CurrentRank = (channelStatus.SMChannel.CurrentRank + 1) % smStreams.Count;
            SMStreamDto smStream = smStreams[channelStatus.SMChannel.CurrentRank];

            if (!streamLimitsService.IsLimited(smStream))
            {
                return smStream;
            }
        }

        return null;
    }

    private void HandleStreamNotFound(IStreamStatus channelStatus)
    {
        logger.LogError("Set Next for Channel {SourceName}, no streams available.", channelStatus.SourceName);
        channelStatus.SetSMStreamInfo(null);
    }

    private bool HandleStreamLimits(IStreamStatus channelStatus, Setting settings)
    {
        if (settings.ShowMessageVideos && cacheManager.MessageNoStreamsLeft != null)
        {
            channelStatus.SetSMStreamInfo(cacheManager.MessageNoStreamsLeft);

            logger.LogDebug("No streams found for {SourceName}, sending message: {Id} {Name}",
                channelStatus.SourceName, cacheManager.MessageNoStreamsLeft.Id, cacheManager.MessageNoStreamsLeft.Name);

            return true;
        }

        logger.LogInformation("Set Next for Channel {SourceName}, no streams found within limits.", channelStatus.SourceName);
        return false;
    }

    private bool HandleCustomPlayListStream(IStreamStatus channelStatus, SMStreamDto smStream, Setting settings)
    {
        CustomPlayList? customPlayList = customPlayListBuilder.GetCustomPlayList(smStream.Name);
        if (customPlayList == null)
        {
            return false;
        }

        (CustomStreamNfo StreamNfo, int SecondsIn) streamNfo = customPlayListBuilder.GetCurrentVideoAndElapsedSeconds(customPlayList.Name);
        SMStreamInfo customStreamInfo = CreateStreamInfoFromCustomPlayList(channelStatus, smStream, settings, streamNfo);

        channelStatus.SetSMStreamInfo(customStreamInfo);

        logger.LogDebug("Set Next for Channel {SourceName}, switched to Custom Playlist {Id} {Name}",
            channelStatus.SourceName, customStreamInfo.Id, customStreamInfo.Name);

        return true;
    }

    private SMStreamInfo CreateStreamInfoFromCustomPlayList(IStreamStatus channelStatus, SMStreamDto smStream, Setting settings, (CustomStreamNfo StreamNfo, int SecondsIn) streamNfo)
    {
        CommandProfileDto customPlayListProfile = profileService.GetCommandProfile("SMFFMPEGLocal");

        return new SMStreamInfo
        {
            Id = streamNfo.StreamNfo.Movie.Title,
            Name = streamNfo.StreamNfo.Movie.Title,
            Url = streamNfo.StreamNfo.VideoFileName,
            SMStreamType = SMStreamTypeEnum.CustomPlayList,
            SecondsIn = streamNfo.SecondsIn,
            ClientUserAgent = GetClientUserAgent(channelStatus.SMChannel, smStream, settings),
            CommandProfile = customPlayListProfile
        };
    }

    private async Task<bool> HandleStandardStreamAsync(IServiceScope scope, IStreamStatus channelStatus, SMStreamDto smStream, Setting settings)
    {
        IStreamGroupService streamGroupService = scope.ServiceProvider.GetRequiredService<IStreamGroupService>();
        CommandProfileDto commandProfile = await streamGroupService.GetProfileFromSGIdsCommandProfileNameAsync(
            null, channelStatus.StreamGroupProfileId, smStream.CommandProfileName ?? channelStatus.SMChannel.CommandProfileName);

        SMStreamInfo standardStreamInfo = new()
        {
            Id = smStream.Id,
            Name = smStream.Name,
            Url = smStream.Url,
            SMStreamType = smStream.SMStreamType,
            ClientUserAgent = GetClientUserAgent(channelStatus.SMChannel, smStream, settings),
            CommandProfile = commandProfile
        };

        channelStatus.SetSMStreamInfo(standardStreamInfo);

        logger.LogDebug("Set Next for Channel {SourceName}, switched to Standard {Id} {Name}",
            channelStatus.SourceName, standardStreamInfo.Id, standardStreamInfo.Name);

        return true;
    }
}
