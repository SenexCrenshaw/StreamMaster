using StreamMaster.PlayList.Models;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Helpers;
using StreamMaster.Streams.Services;

using System.Threading.Channels;

namespace StreamMaster.Streams.Handlers;

public sealed class ChannelBroadcaster(ILogger<IChannelBroadcaster> logger, SMChannelDto smChannelDto, int streamGroupProfileId, bool remux)
    : BroadcasterBase(logger), IChannelBroadcaster, IDisposable
{
    public event EventHandler<ChannelBroascasterStopped>? OnChannelBroadcasterStoppedEvent;
    private readonly Dubcer _dubcer = new(logger);
    /// <inheritdoc/>
    public int Id { get; }
    public override string StringId()
    {
        return Id.ToString();
    }

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

    public void SetSourceChannelBroadcaster(ISourceBroadcaster SourceChannelBroadcaster)
    {
        Channel<byte[]> channel = ChannelHelper.GetChannel();
        //Channel<byte[]> dubcerChannel = ChannelHelper.GetChannel();

        SourceChannelBroadcaster.AddChannelStreamer(SMChannel.Id, channel.Writer);

        //dubcer.DubcerChannels(channel.Reader, dubcerChannel.Writer, CancellationToken.None);

        if (remux)
        {
            Dubcer ??= new(logger);
            Dubcer.SetSourceChannel(channel.Reader);
            channel = Dubcer.DubcerChannel;
        }

        SetSourceChannel(channel.Reader, SourceChannelBroadcaster.Name, channel, CancellationToken.None);
    }

    public void Dispose()
    {
        Stop();
    }
}
