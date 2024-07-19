using AutoMapper;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.Profiles.Queries;
using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;
using StreamMaster.PlayList;
using StreamMaster.PlayList.Models;
using StreamMaster.Streams.Domain.Extensions;

using System.Collections.Concurrent;
namespace StreamMaster.Streams.Channels;

public sealed class ChannelService(
    ILogger<ChannelService> logger,
    IStreamManager streamManager,
    IClientStreamerManager clientStreamerManager,
    IOptionsMonitor<CommandProfileList> intProfileSettings,
    IServiceProvider serviceProvider,
    IMapper mapper,
    ICustomPlayListBuilder customPlayListBuilder,
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

    public async Task<IChannelStatus?> RegisterChannel(ClientStreamerConfiguration config)
    {
        if (config.SMChannel == null)
        {
            throw new ArgumentNullException(nameof(config.SMChannel));
        }

        IChannelStatus? channelStatus = GetChannelStatus(config.SMChannel.Id);

        _ = clientStreamerManager.RegisterClient(config);

        CommandProfileList profileSettings = intProfileSettings.CurrentValue;

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

            channelStatus.CommandProfile = profileSettings.CommandProfiles.FirstOrDefault(a => a.Key == channelStatus.SMChannel.CommandProfileName).Value.ToCommandProfileDto(channelStatus.SMChannel.CommandProfileName);

            if (handler.IsFailed)
            {
                logger.LogInformation("Existing handler is failed, creating");

                _ = await SwitchChannelToNextStream(channelStatus);
            }

            streamManager.AddClientToHandler(config, handler);
            logger.LogInformation("Reuse existing stream handler for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.SMChannel.Id, config.SMChannel.Name);

            return channelStatus;
        }

        logger.LogInformation("No existing channel for {ClientId} {ChannelVideoStreamId} {name}", config.ClientId, config.SMChannel.Id, config.SMChannel.Name);

        CustomPlayList? customPlayList = null;
        if (config.SMChannel.IsCustomStream)
        {
            customPlayList = customPlayListBuilder.GetCustomPlayList(config.SMChannel.Name);
        }


        using IServiceScope scope = _serviceProvider.CreateScope();
        ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();

        DataResponse<CommandProfileDto> commandProfileData = await sender.Send(new GetCommandProfileRequest(BuildInfo.DefaultCommandProfileName, config.StreamGroupId, config.StreamGroupProfileId));

        if (commandProfileData.Data == null)
        {

            logger.LogError("Could not find video profile for {CommandProfileName}", config.SMChannel.CommandProfileName);
            return null;
        }

        CommandProfileDto? commandProfile = commandProfileData.Data;
        string profileName = commandProfile.ProfileName;

        channelStatus = new ChannelStatus(config.SMChannel)
        {
            StreamGroupProfileId = config.StreamGroupProfileId,
            CommandProfile = commandProfileData.Data,
            CustomPlayList = customPlayList
        };

        _ = _channelStatuses.TryAdd(config.SMChannel.Id, channelStatus);

        //await SetNextChildVideoStream(channelStatus).ConfigureAwait(false);
        _ = await SwitchChannelToNextStream(channelStatus);

        _ = channelStreamingStatisticsManager.RegisterInputReader(channelStatus.SMChannel, channelStatus.CurrentRank, channelStatus.SMStream.Id);

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
                CommandProfile = new CommandProfileDto()
            };
            _ = _channelStatuses.TryAdd(smChannel.Id, channelStatus);

            _ = await SetNextChildVideoStream(channelStatus).ConfigureAwait(false);
        }

        return channelStatus;
    }

    public void UnRegisterChannel(int smChannelId)
    {
        _ = _channelStatuses.TryRemove(smChannelId, out _);
        channelStreamingStatisticsManager.DecrementClient(smChannelId);
        //channelStreamingStatisticsManager.UnRegister(smChannelId);
    }

    public IChannelStatus? GetChannelStatus(int smChannelId)
    {
        _ = _channelStatuses.TryGetValue(smChannelId, out IChannelStatus? channelStatus);
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
        _ = streamManager.GetStreamHandler(channelStatus.SMStream?.Url);

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
        if (channelStatus.SMChannel.IsCustomStream)
        {
            if (channelStatus.CustomPlayList == null)
            {
                return false;
            }

            if (channelStatus.CurrentRank > -1)
            {
                channelStatus.CurrentRank++;

                if (channelStatus.CurrentRank >= channelStatus.CustomPlayList.CustomStreamNfos.Count)
                {
                    channelStatus.CurrentRank = 0;
                }
                return true;
            }
        }

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
