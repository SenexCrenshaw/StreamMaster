using StreamMaster.PlayList.Models;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Helpers;

using System.Threading.Channels;

namespace StreamMaster.Streams.Handlers;

public sealed class ChannelBroadcaster : BroadcasterBase, IChannelBroadcaster, IDisposable
{
    private readonly ILogger<IChannelBroadcaster> logger;
    public ChannelBroadcaster(ILogger<IChannelBroadcaster> logger, SMChannelDto smChannelDto) : base(logger)
    {
        Id = smChannelDto.Id;
        Name = smChannelDto.Name;
        this.logger = logger;
    }


    public event EventHandler<ChannelBroascasterStopped>? OnChannelStatusStoppedEvent;

    /// <inheritdoc/>
    public int Id { get; }
    public override string StringId()
    {
        return Id.ToString();
    }



    /// <inheritdoc/>
    public override void OnStreamingStopped()
    {
        OnChannelStatusStoppedEvent?.Invoke(this, new ChannelBroascasterStopped(Id, Name));
    }

    public int IntroIndex { get; set; }
    public bool PlayedIntro { get; set; }
    public bool IsFirst { get; set; } = true;
    //public required int StreamGroupId { get; set; }
    public required int StreamGroupProfileId { get; set; }
    public required SMChannelDto SMChannel { get; set; }
    public bool Shutdown { get; set; } = false;
    public bool IsStarted { get; set; }
    public SMStreamInfo? SMStreamInfo { get; private set; }
    public bool IsGlobal { get; set; }
    public bool FailoverInProgress { get; set; }
    public string? OverrideSMStreamId { get; set; } = null;
    public string ClientUserAgent { get; set; } = string.Empty;
    public CustomPlayList? CustomPlayList { get; set; }

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

    public void SetSourceChannelBroadcaster(IStreamBroadcaster SourceChannelBroadcaster)
    {
        Channel<byte[]> channel = ChannelHelper.GetChannel();
        //Channel<byte[]> dubcerChannel = ChannelHelper.GetChannel();

        SourceChannelBroadcaster.AddClientChannel(SMChannel.Id, channel.Writer);

        //dubcer.DubcerChannels(channel.Reader, dubcerChannel.Writer, CancellationToken.None);

        SetSourceChannel(channel.Reader, SourceChannelBroadcaster.Name, CancellationToken.None);
        //SetSourceChannel(dubcerChannel.Reader, SourceChannelBroadcaster.Name, CancellationToken.None);
    }

    public void Dispose()
    {
        Stop();
    }
}
