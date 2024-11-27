using System.Threading.Channels;

using StreamMaster.PlayList.Models;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Helpers;
using StreamMaster.Streams.Services;

namespace StreamMaster.Streams.Handlers;

public sealed class ChannelBroadcaster(ILogger<IChannelBroadcaster> Logger, IOptionsMonitor<Setting> Settings, SMChannelDto smChannelDto, int streamGroupProfileId, bool remux)
    : BroadcasterBase(Logger, Settings), IChannelBroadcaster, IDisposable
{
    public event EventHandler<ChannelBroascasterStopped>? OnChannelBroadcasterStoppedEvent;

    /// <inheritdoc/>
    public int Id => smChannelDto.Id;

    public override string StringId()
    {
        return Id.ToString();
    }

    public override string Name => smChannelDto.Name;

    public override void Stop()
    {
        // Derived-specific logic before stopping
        logger.LogInformation("Channel Broadcaster stopped: {Name}", Name);

        // Call base class stop logic
        base.Stop();

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

    private Dubcer? Dubcer = null;

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

    public void SetSourceChannelBroadcaster(IBroadcasterBase SourceChannelBroadcaster)
    {
        if (remux)
        {
            Dubcer ??= new(logger);
            SourceChannelBroadcaster.AddChannelStreamer(SMChannel.Id, Dubcer.DubcerChannel.Writer);
            SetSourceChannel(Dubcer.DubcerChannel.Reader, SourceChannelBroadcaster.Name, CancellationToken.None);
        }
        else
        {
            Channel<byte[]> channel = ChannelHelper.GetChannel();
            SourceChannelBroadcaster.AddChannelStreamer(SMChannel.Id, channel.Writer);
            SetSourceChannel(channel.Reader, SourceChannelBroadcaster.Name, CancellationToken.None);
        }
    }

    public void Dispose()
    {
        Stop();
    }
}