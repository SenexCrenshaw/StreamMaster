using AutoMapper;

using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Configuration;

using System.Collections.Concurrent;
namespace StreamMaster.Streams.Channels;

public sealed class ChannelService(
    ILogger<ChannelService> logger,
    IStreamManager streamManager,
    IClientStreamerManager clientStreamerManager,
    IOptionsMonitor<VideoOutputProfiles> intProfileSettings,
    IServiceProvider serviceProvider,
    IMapper mapper,
    IOptionsMonitor<Setting> settingsMonitor,
    IChannelStreamingStatisticsManager channelStreamingStatisticsManager
    ) : IChannelService, IDisposable
{
    private readonly ILogger<ChannelService> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    private readonly ConcurrentDictionary<int, IChannelStatus> _channelStatuses = new();
    private readonly object _disposeLock = new();
    private bool _disposed = false;

    public void Dispose()
    {
        lock (_disposeLock)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                _channelStatuses.Clear();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while disposing the ChannelService.");
            }
            finally
            {
                _disposed = true;
            }
        }
    }

    private VideoOutputProfileDto VideoOutputProfileDto(string StreamingProxyType)
    {
        return intProfileSettings.CurrentValue.VideoProfiles.TryGetValue(StreamingProxyType, out VideoOutputProfile? videoOutputProfile)
            ? new VideoOutputProfileDto
            {
                Command = videoOutputProfile.Command,
                ProfileName = StreamingProxyType,
                IsReadOnly = videoOutputProfile.IsReadOnly,
                Parameters = videoOutputProfile.Parameters,
                Timeout = videoOutputProfile.Timeout,
                IsM3U8 = videoOutputProfile.IsM3U8
            }
            : new VideoOutputProfileDto
            {
                ProfileName = StreamingProxyType,
            };
    }
    public async Task<IChannelStatus?> RegisterChannel(ClientStreamerConfiguration config)
    {

        if (config.SMChannel == null)
        {
            throw new ArgumentNullException(nameof(config.SMChannel));
        }

        IChannelStatus? channelStatus = GetChannelStatus(config.SMChannel.Id);

        clientStreamerManager.RegisterClient(config);

        if (channelStatus != null)
        {
            IStreamHandler? handler = streamManager.GetStreamHandler(channelStatus.SMStream.Url);
            if (handler is null)
            {
                logger.LogError("Could not find handler for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.SMChannel.Id, config.SMChannel.Name);
                await clientStreamerManager.UnRegisterClient(config.ClientId);
                UnRegisterChannel(config.SMChannel.Id);
                return null;
            }
            channelStatus.VideoProfile = VideoOutputProfileDto(config.SMChannel.StreamingProxyType);

            if (handler.IsFailed)
            {
                logger.LogInformation("Existing handler is failed, creating");

                await SwitchChannelToNextStream(channelStatus);
            }

            streamManager.AddClientToHandler(config, handler);
            logger.LogInformation("Reuse existing stream handler for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.SMChannel.Id, config.SMChannel.Name);

            return channelStatus;
        }

        logger.LogInformation("No existing channel for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.SMChannel.Id, config.SMChannel.Name);

        channelStatus = new ChannelStatus(config.SMChannel)
        {
            VideoProfile = VideoOutputProfileDto(config.SMChannel.StreamingProxyType)
        };

        _channelStatuses.TryAdd(config.SMChannel.Id, channelStatus);

        //await SetNextChildVideoStream(channelStatus).ConfigureAwait(false);
        await SwitchChannelToNextStream(channelStatus);

        channelStreamingStatisticsManager.RegisterInputReader(channelStatus.SMChannel, channelStatus.CurrentRank, channelStatus.SMStream.Id);

        return channelStatus;
    }


    public async Task<IChannelStatus?> SetupChannel(SMChannelDto smChannel)
    {
        if (smChannel == null)
        {
            throw new ArgumentNullException(nameof(smChannel));
        }

        IChannelStatus? channelStatus = GetChannelStatus(smChannel.Id);
        if (channelStatus == null)
        {
            channelStatus = new ChannelStatus(smChannel)
            {
                VideoProfile = VideoOutputProfileDto(smChannel.StreamingProxyType)
            };
            _channelStatuses.TryAdd(smChannel.Id, channelStatus);

            await SetNextChildVideoStream(channelStatus).ConfigureAwait(false);

        }

        return channelStatus;
    }

    public void UnRegisterChannel(int smChannelId)
    {
        _channelStatuses.TryRemove(smChannelId, out _);
        channelStreamingStatisticsManager.DecrementClient(smChannelId);
        //channelStreamingStatisticsManager.UnRegister(smChannelId);
    }

    private List<VideoOutputProfileDto> GetProfiles()
    {

        List<VideoOutputProfileDto> ret = [];

        foreach (string key in intProfileSettings.CurrentValue.VideoProfiles.Keys)
        {
            ret.Add(new VideoOutputProfileDto
            {
                ProfileName = key,
                IsReadOnly = intProfileSettings.CurrentValue.VideoProfiles[key].IsReadOnly,
                Parameters = intProfileSettings.CurrentValue.VideoProfiles[key].Parameters,
                Timeout = intProfileSettings.CurrentValue.VideoProfiles[key].Timeout,
                IsM3U8 = intProfileSettings.CurrentValue.VideoProfiles[key].IsM3U8
            });
        }

        return ret;
    }

    public IChannelStatus? GetChannelStatus(int smChannelId)
    {
        _channelStatuses.TryGetValue(smChannelId, out IChannelStatus? channelStatus);
        return channelStatus;
    }

    public List<IChannelStatus> GetChannelStatusesFromSMStreamId(string StreamId)
    {
        try
        {
            if (StreamId is null)
            {
                logger.LogError("StreamId is null");
                return [];
            }
            return _channelStatuses.Values.Where(a => a.SMStream.Id == StreamId).ToList();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while getting channel statuses from SMStream ID {StreamId}.", StreamId);
            return [];
        }
    }

    public IChannelStatus? GetChannelStatusFromSMChannelId(int smChannelId)
    {
        return _channelStatuses.TryGetValue(smChannelId, out IChannelStatus? channelStatus) ? channelStatus : null;
    }

    public List<IChannelStatus> GetChannelStatuses()
    {
        return [.. _channelStatuses.Values];
    }

    public bool HasChannel(int smChannelId)
    {
        return _channelStatuses.ContainsKey(smChannelId);
    }

    public int GetGlobalStreamsCount()
    {
        return _channelStatuses.Values.Count(a => a.IsGlobal);
    }
    public async Task<bool> SwitchChannelToNextStream(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null)
    {
        if (channelStatus.FailoverInProgress)
        {
            return false;
        }

        channelStatus.FailoverInProgress = true;
        if (!string.IsNullOrEmpty(overrideNextVideoStreamId))
        {
            channelStatus.OverrideVideoStreamId = overrideNextVideoStreamId;
        }

        logger.LogDebug("Starting SwitchToNextVideoStream with channelStatus: {channelStatus} and overrideNextVideoStreamId: {overrideNextVideoStreamId}", channelStatus, overrideNextVideoStreamId);

        IStreamHandler? oldStreamHandler = streamManager.GetStreamHandler(channelStatus.SMStream?.Url);

        bool didChange = await SetNextChildVideoStream(channelStatus, overrideNextVideoStreamId);

        if (channelStatus.SMStream is null || !didChange)
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to smStream being null");
            channelStatus.FailoverInProgress = false;
            return false;
        }

        IStreamHandler? newStreamHandler = await streamManager.GetOrCreateStreamHandler(channelStatus);

        if (newStreamHandler is null)
        {
            logger.LogDebug("Exiting SwitchToNextVideoStream with false due to channelStatus. newStreamHandler is null");
            channelStatus.FailoverInProgress = false;

            return false;
        }


        List<ClientStreamerConfiguration> clientConfigs = clientStreamerManager.GetClientStreamerConfigurationsBySMChannelId(channelStatus.SMChannel.Id);
        streamManager.AddClientsToHandler(clientConfigs, newStreamHandler);


        channelStatus.FailoverInProgress = false;

        logger.LogDebug("Finished SwitchToNextVideoStream");
        return true;
    }


    public async Task<bool> SetNextChildVideoStream(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null)
    {
        Setting _settings = settingsMonitor.CurrentValue ?? throw new ArgumentNullException(nameof(settingsMonitor));
        using IServiceScope scope = _serviceProvider.CreateScope();
        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

        Dictionary<int, M3UFileDto> m3uFilesRepo = (await repository.M3UFile.GetM3UFiles().ConfigureAwait(false))
                            .ToDictionary(m => m.Id);

        if (!string.IsNullOrEmpty(overrideNextVideoStreamId))
        {
            SMStreamDto? smStream = repository.SMStream.GetSMStream(overrideNextVideoStreamId);
            if (smStream == null)
            {
                return false;
            }

            if (!m3uFilesRepo.TryGetValue(smStream.M3UFileId, out M3UFileDto? m3uFile))
            {
                if (GetGlobalStreamsCount() >= _settings.GlobalStreamLimit)
                {
                    logger.LogInformation("Max global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), smStream.Url);
                    return false;
                }

                channelStatus.SetIsGlobal();
                logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
            }
            else if (m3uFile.MaxStreamCount > 0)
            {
                int allStreamsCount = GetCurrentStreamCountForM3UFile(m3uFile.Id);
                if (allStreamsCount >= m3uFile.MaxStreamCount)
                {
                    logger.LogInformation("Max stream count {AllStreamsCount}/{MaxStreams} reached for stream: {StreamUrl}", allStreamsCount, m3uFile.MaxStreamCount, smStream.Url);
                    return false;
                }
            }

            logger.LogDebug("Exiting SetNextChildVideoStream with to Return: {Id} {Name}", smStream.Id, smStream.Name);
            channelStatus.SetCurrentSMStream(smStream);
            return false;
        }

        SMChannel? channel = repository.SMChannel.GetSMChannel(channelStatus.SMChannel.Id);
        if (channel == null)
        {
            logger.LogError("SetNextChildVideoStream could not get videoStreams for id {ParentVideoStreamId}", channelStatus.SMChannel.Id);
            //channelStatus.SetCurrentSMStream(null);
            return false;
        }

        if (channel.SMStreams.Count == 0)
        {
            logger.LogDebug("Exiting SetNextChildVideoStream with null due to no suitable videoStream found");
            //channelStatus.SetCurrentSMStream(null);
            return false;
        }

        if (channelStatus.CurrentRank + 1 >= channel.SMStreams.Count)
        {
            logger.LogInformation("SetNextChildVideoStream no more streams for id {ParentVideoStreamId}, exiting", channelStatus.SMChannel.Id);
            //channelStatus.SetCurrentSMStream(null);
            return false;
        }


        List<SMStream> smStreams = channel.SMStreams.OrderBy(a => a.Rank).Select(a => a.SMStream).ToList();

        for (int i = 0; i < smStreams.Count; i++)
        {
            channelStatus.CurrentRank = (channelStatus.CurrentRank + 1) % smStreams.Count;
            SMStream toReturn = smStreams[channelStatus.CurrentRank];

            if (!m3uFilesRepo.TryGetValue(toReturn.M3UFileId, out M3UFileDto? m3uFile))
            {
                if (GetGlobalStreamsCount() >= _settings.GlobalStreamLimit)
                {
                    logger.LogInformation("Max global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), toReturn.Url);
                    continue;
                }

                channelStatus.SetIsGlobal();
                logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
            }
            else if (m3uFile.MaxStreamCount > 0)
            {
                int allStreamsCount = GetCurrentStreamCountForM3UFile(m3uFile.Id);
                if (allStreamsCount >= m3uFile.MaxStreamCount)
                {
                    logger.LogInformation("Max stream count {AllStreamsCount}/{MaxStreams} reached for stream: {StreamUrl}", allStreamsCount, m3uFile.MaxStreamCount, toReturn.Url);
                    continue;
                }
            }

            logger.LogDebug("Exiting SetNextChildVideoStream with to Return: {Id} {Name}", toReturn.Id, toReturn.Name);
            SMStreamDto a = mapper.Map<SMStreamDto>(toReturn);
            channelStatus.SetCurrentSMStream(a);
            return true;
        }
        return false;
    }

    // Helper method to get current stream count for a specific M3U file
    private int GetCurrentStreamCountForM3UFile(int m3uFileId)
    {
        return 0;
    }

}
