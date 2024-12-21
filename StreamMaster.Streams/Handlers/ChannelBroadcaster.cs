using System.Collections.Concurrent;
using System.IO.Pipelines;

using StreamMaster.PlayList.Models;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Services;

namespace StreamMaster.Streams.Handlers;

public sealed class ChannelBroadcaster(ILogger<IChannelBroadcaster> logger, SMChannelDto smChannelDto, int streamGroupProfileId)
    : IChannelBroadcaster, IDisposable
{
    public event EventHandler<ChannelBroascasterStopped>? OnChannelBroadcasterStoppedEvent;

    public ConcurrentDictionary<string, IClientConfiguration> Clients { get; } = new();
    public bool ClientConfigurationsEmpty => Clients.IsEmpty || !Clients.Keys.Any(a => a != "VideoInfo");

    private readonly Task? _streamingTask;
    private readonly SemaphoreSlim _streamLock = new(1, 1);

    //public Pipe Pipe { get; set; } = new();

    /// <inheritdoc/>
    public int Id => smChannelDto.Id;

    public string StringId()
    {
        return Id.ToString();
    }

    private ISourceBroadcaster? SourceBroadcaster { get; set; }

    public string Name => smChannelDto.Name;

    public void AddChannelStreamer(IClientConfiguration clientConfiguration)
    {
        Clients.TryAdd(clientConfiguration.UniqueRequestId, clientConfiguration);
    }

    public async Task StreamDataToClientsAsync(System.Buffers.ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
    {
        IEnumerable<Task> tasks = Clients.Values.Select(async client =>
        {
            try
            {
                foreach (ReadOnlyMemory<byte> segment in buffer)
                {
                    await client.Pipe.Writer.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
                }

                await client.Pipe.Writer.FlushAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error writing to client. Stopping client {ClientId}.", client.UniqueRequestId);
                client.Stop();
                Clients.TryRemove(client.UniqueRequestId, out _);
            }
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public bool RemoveClient(string clientId)
    {
        if (Clients.TryRemove(clientId, out IClientConfiguration? client))
        {
            client.Stop();
            return true;
        }
        return false;
    }

    public void Stop()
    {
        // Derived-specific logic before stopping
        logger.LogInformation("Channel Broadcaster stopped: {Name}", Name);

        Dubcer?.Stop();

        // Additional cleanup or finalization
        OnChannelBroadcasterStoppedEvent?.Invoke(this, new ChannelBroascasterStopped(Id, Name));
    }

    public int IntroIndex { get; set; }
    public bool PlayedIntro { get; set; }
    public bool IsFirst { get; set; } = true;
    public int StreamGroupProfileId => streamGroupProfileId;
    public SMChannelDto SMChannel => smChannelDto;
    public bool Shutdown { get; set; } = false;
    public SMStreamInfo? SMStreamInfo { get; private set; }
    public bool IsGlobal { get; set; }
    public bool FailoverInProgress { get; set; }
    public string? OverrideSMStreamId { get; set; } = null;
    public CustomPlayList? CustomPlayList { get; set; }

    public string SourceName => SMStreamInfo?.Name ?? "";

    public bool IsFailed { get; } = false;

    private readonly Dubcer? Dubcer = null;

    public void SetSMStreamInfo(SMStreamInfo? smStreamInfo)
    {
        if (smStreamInfo == null)
        {
            logger.LogDebug("SetSMStreamInfo null");
        }
        else
        {
            logger.LogDebug("SetSMStreamInfo: {Id} {Name} {Url}", smStreamInfo.Id, smStreamInfo.Name, smStreamInfo.Url);
        }

        SMStreamInfo = smStreamInfo;
    }

    public void SetIsGlobal()
    {
        IsGlobal = true;
    }

    public void SetSourceChannelBroadcaster(ISourceBroadcaster SourceChannelBroadcaster)
    {
        //if (remux)
        //{
        //    Dubcer ??= new(logger);
        //    SourceChannelBroadcaster.AddChannelStreamer(SMChannel.Id, Dubcer.DubcerChannel.Writer);
        //    SetSourceChannel(Dubcer.DubcerChannel.Reader, SourceChannelBroadcaster.Name, CancellationToken.None);
        //}
        //else
        //{
        //    Channel<byte[]> channel = ChannelHelper.GetChannel();
        //    SourceChannelBroadcaster.AddChannelStreamer(SMChannel.Id, channel.Writer);
        //    SetSourceChannel(channel.Reader, SourceChannelBroadcaster.Name, CancellationToken.None);
        //}

        SourceBroadcaster?.RemoveChannelBroadcaster(Id);
        SourceBroadcaster = SourceChannelBroadcaster;
        SourceChannelBroadcaster.AddChannelBroadcaster(this);
    }
    public void Dispose()
    {
        Stop();
    }
}