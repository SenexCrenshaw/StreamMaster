using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Domain.Configuration;
using StreamMaster.Streams.Channels;

using System.Collections.Concurrent;

public sealed class ChannelService : IChannelService, IDisposable
{
    private readonly ILogger<ChannelService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Setting _settings;
    private readonly ConcurrentDictionary<int, IChannelStatus> _channelStatuses = new();
    private readonly object _disposeLock = new();
    private bool _disposed = false;

    public ChannelService(ILogger<ChannelService> logger, IServiceProvider serviceProvider, IOptionsMonitor<Setting> settingsMonitor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _settings = settingsMonitor.CurrentValue ?? throw new ArgumentNullException(nameof(settingsMonitor));
    }

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
                _logger.LogError(ex, "An error occurred while disposing the ChannelService.");
            }
            finally
            {
                _disposed = true;
            }
        }
    }

    public async Task<IChannelStatus?> RegisterChannel(SMChannel smChannel, bool fetch = false)
    {
        if (smChannel == null)
        {
            throw new ArgumentNullException(nameof(smChannel));
        }

        IChannelStatus? channelStatus = GetChannelStatus(smChannel.Id);
        if (channelStatus == null)
        {
            channelStatus = new ChannelStatus(smChannel);
            _channelStatuses.TryAdd(smChannel.Id, channelStatus);

            if (fetch)
            {
                await SetNextChildVideoStream(smChannel.Id).ConfigureAwait(false);
            }
        }

        return channelStatus;
    }

    public void UnRegisterChannel(int smChannelId)
    {
        _channelStatuses.TryRemove(smChannelId, out _);
    }

    public IChannelStatus? GetChannelStatus(int smChannelId)
    {
        _channelStatuses.TryGetValue(smChannelId, out IChannelStatus? channelStatus);
        return channelStatus;
    }

    public List<IChannelStatus> GetChannelStatusesFromSMStreamId(string StreamId)
    {
        return _channelStatuses.Values.Where(a => a.SMStream.Id == StreamId).ToList();
    }

    public List<IChannelStatus> GetChannelStatusesFromSMChannelId(int smChannelId)
    {
        return _channelStatuses.Values.Where(a => a.Id == smChannelId).ToList();
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

    public async Task SetNextChildVideoStream(int smChannelId, string? overrideNextVideoStreamId = null)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        IRepositoryWrapper repository = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();

        IChannelStatus? channelStatus = GetChannelStatus(smChannelId);
        if (channelStatus == null)
        {
            _logger.LogError("Channel status not found for channel ID {ChannelId}", smChannelId);
            return;
        }


        List<M3UFileDto> m3uFilesRepo = await repository.M3UFile.GetM3UFiles().ConfigureAwait(false);


        if (!string.IsNullOrEmpty(overrideNextVideoStreamId))
        {
            SMStream? vs = repository.SMStream.GetSMStream(overrideNextVideoStreamId);
            if (vs == null)
            {
                return;
            }

            M3UFileDto? m3uFile = m3uFilesRepo.Find(a => a.Id == vs.M3UFileId);
            if (m3uFile == null)
            {
                if (GetGlobalStreamsCount() >= _settings.GlobalStreamLimit)
                {
                    _logger.LogInformation("Max global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), vs.Url);
                    return;
                }

                channelStatus.SetIsGlobal();
                _logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
            }
            else
            {
                int allStreamsCount = 0; // replace with actual logic
                if (allStreamsCount >= m3uFile.MaxStreamCount)
                {
                    _logger.LogInformation("Max stream count {AllStreamsCount}/{MaxStreams} reached for stream: {StreamUrl}", allStreamsCount, m3uFile.MaxStreamCount, vs.Url);
                    return;
                }
            }

            _logger.LogDebug("Exiting SetNextChildVideoStream with to Return: {Id} {Name}", vs.Id, vs.Name);
            channelStatus.SetCurrentSMStream(vs);
            return;


        }


        SMChannel? channel = repository.SMChannel.GetSMChannel(channelStatus.Id);
        if (channel == null)
        {
            _logger.LogError("SetNextChildVideoStream could not get videoStreams for id {ParentVideoStreamId}", channelStatus.Id);
            _logger.LogDebug("Exiting SetNextChildVideoStream with null due to result being null");
            channelStatus.SetCurrentSMStream(null);
            return;
        }

        List<SMStream> smStreams = channel.SMStreams.OrderBy(s => s.Rank).Select(a => a.SMStream).ToList();
        if (!smStreams.Any())
        {
            channelStatus.SetCurrentSMStream(null);
            return;
        }

        if (channelStatus.Rank >= smStreams.Count)
        {
            channelStatus.Rank = 0;
        }

        while (channelStatus.Rank < smStreams.Count)
        {
            SMStream toReturn = smStreams[channelStatus.Rank++];
            M3UFileDto? m3uFile = m3uFilesRepo.Find(a => a.Id == toReturn.M3UFileId);
            if (m3uFile == null)
            {
                if (GetGlobalStreamsCount() >= _settings.GlobalStreamLimit)
                {
                    _logger.LogInformation("Max global stream count {GlobalStreamsCount} reached for stream: {StreamUrl}", GetGlobalStreamsCount(), toReturn.Url);
                    continue;
                }

                channelStatus.SetIsGlobal();
                _logger.LogInformation("Global stream count {GlobalStreamsCount}", GetGlobalStreamsCount());
            }
            else
            {
                int allStreamsCount = 0; // replace with actual logic
                if (allStreamsCount >= m3uFile.MaxStreamCount)
                {
                    _logger.LogInformation("Max stream count {AllStreamsCount}/{MaxStreams} reached for stream: {StreamUrl}", allStreamsCount, m3uFile.MaxStreamCount, toReturn.Url);
                    continue;
                }
            }

            _logger.LogDebug("Exiting SetNextChildVideoStream with to Return: {Id} {Name}", toReturn.Id, toReturn.Name);
            channelStatus.SetCurrentSMStream(toReturn);
            return;
        }

        _logger.LogDebug("Exiting SetNextChildVideoStream with null due to no suitable videoStream found");
        channelStatus.SetCurrentSMStream(null);
    }
}
