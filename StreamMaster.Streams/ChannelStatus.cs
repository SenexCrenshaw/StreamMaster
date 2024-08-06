using StreamMaster.PlayList.Models;
using StreamMaster.Streams.Domain.Helpers;

using System.Threading.Channels;

namespace StreamMaster.Streams;

public sealed class ChannelStatus : ChannelStatusBroadcaster, IChannelStatus, IDisposable
{
    private readonly ILogger<IChannelBroadcaster> logger;

    public ChannelStatus(ILogger<IChannelBroadcaster> logger, SMChannelDto smChannelDto) : base(logger, smChannelDto)
    {
        this.logger = logger;
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

    public void SetSourceChannelBroadcaster(IChannelBroadcaster SourceChannelBroadcaster)
    {
        Channel<byte[]> channel = ChannelHelper.GetChannel(ChannelHelper.DefaultChannelCapacity);
        SourceChannelBroadcaster.AddClientChannel(SMChannel.Id, channel.Writer);
        SetSourceChannel(channel.Reader, SourceChannelBroadcaster.Name, CancellationToken.None);
    }

    public void Dispose()
    {
        Stop();
    }
}