using StreamMaster.PlayList.Models;
using StreamMaster.Streams.Domain.Helpers;

using System.Collections.Concurrent;
using System.Threading.Channels;

namespace StreamMaster.Streams;

public sealed class ChannelStatus : IntroStatus, IChannelStatus, IDisposable
{
    private readonly IChannelDistributorService channelDistributorService;
    private readonly IVideoInfoService VideoInfoService;

    public ChannelStatus(ILogger<IChannelStatus> logger, IVideoInfoService VideoInfoService, IChannelDistributorService channelDistributorService, IDubcer Dubcer, SMChannelDto smChannel)
    {
        this.VideoInfoService = VideoInfoService;
        this.channelDistributorService = channelDistributorService;
        SMChannel = smChannel;
        CurrentRank = smChannel.CurrentRank;

        this.Dubcer = Dubcer;
    }


    public IDubcer Dubcer { get; }
    public SMChannelDto SMChannel { get; }
    private ConcurrentDictionary<string, IClientConfiguration> ClientStreamerConfigurations { get; } = new();
    public bool Shutdown { get; set; } = false;
    public bool IsStarted { get; set; }
    public SMStreamInfo? SMStreamInfo { get; private set; }
    public bool IsGlobal { get; set; }
    public bool FailoverInProgress { get; set; }
    public int CurrentRank { get; set; }
    public string? OverrideSMStreamId { get; set; } = null;
    public string ClientUserAgent { get; set; } = string.Empty;
    public int ClientCount => ClientStreamerConfigurations.Keys.Count;

    public int StreamGroupId { get; set; }
    public int StreamGroupProfileId { get; set; }
    public CommandProfileDto CommandProfile { get; set; }
    public CustomPlayList? CustomPlayList { get; set; }

    public IChannelDistributor ChannelDistributor { get; set; }

    public List<IClientConfiguration> GetClientStreamerConfigurations()
    {
        return [.. ClientStreamerConfigurations.Values];
    }

    /// <summary>
    /// Adds a client streamer configuration to the dictionary.
    /// </summary>
    /// <param name="UniqueRequestId">The client identifier.</param>
    /// <param name="config">The client streamer configuration to add.</param>
    public void AddClient(string UniqueRequestId, IClientConfiguration config)
    {
        ClientStreamerConfigurations[UniqueRequestId] = config;
        ChannelDistributor.AddClientChannel(UniqueRequestId, config.ClientStream.Channel);
    }

    /// <summary>
    /// Removes a client streamer configuration from the dictionary.
    /// </summary>
    /// <param name="UniqueRequestId">The client identifier.</param>
    public bool RemoveClient(string UniqueRequestId)
    {
        if (ClientStreamerConfigurations.TryRemove(UniqueRequestId, out IClientConfiguration? _))
        {
            ChannelDistributor.RemoveClientChannel(UniqueRequestId);
            return true;
        }

        return false;
    }

    public void SetSMStreamInfo(SMStreamInfo? idNameUrl)
    {
        SMStreamInfo = idNameUrl;
    }

    public void SetIsGlobal()
    {
        IsGlobal = true;
    }

    public void SetSourceChannel(IChannelDistributor sourceChannelDistributor, string Name, bool IsCustom)
    {
        Channel<byte[]> channel = ChannelHelper.GetChannel(IsCustom);


        sourceChannelDistributor.AddClientChannel(SMChannel.Id, channel.Writer);

        if (!sourceChannelDistributor.SMStreamInfo.Id.StartsWith(IntroPlayListBuilder.IntroIDPrefix, StringComparison.InvariantCulture))
        {
            Channel<byte[]> channelVideoInfo = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(200) { SingleReader = true, SingleWriter = true, FullMode = BoundedChannelFullMode.DropOldest });
            sourceChannelDistributor.AddClientChannel("VideoInfo " + SMChannel.Id, channelVideoInfo.Writer);
            VideoInfoService.SetSourceChannel(channelVideoInfo.Reader, Name);
        }
        //Dubcer.DubcerChannels(channel.Reader, channelMux.Writer, CancellationToken.None);
        ChannelDistributor.SetSourceChannel(channel.Reader, Name, CancellationToken.None);
    }

    public void Dispose()
    {
        ChannelDistributor.Stop();
    }
}