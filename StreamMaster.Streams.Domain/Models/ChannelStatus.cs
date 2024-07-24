using StreamMaster.Domain.Configuration;
using StreamMaster.PlayList.Models;

using System.Collections.Concurrent;
using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Models;

public sealed class ChannelStatus(SMChannelDto smChannel) : IChannelStatus
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Channel<byte[]> _currentChannel = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });
    public ConcurrentDictionary<string, ClientStreamerConfiguration> ClientStreamerConfigurations { get; set; } = new();
    public bool Shutdown { get; set; } = false;
    public bool IsStarted { get; set; }
    public bool IsFirst { get; set; } = true;
    public bool IsGlobal { get; set; }
    public bool FailoverInProgress { get; set; }
    public SMStreamDto SMStream { get; private set; }
    public SMChannelDto SMChannel => smChannel;
    public int CurrentRank { get; set; } = smChannel.CurrentRank;
    public int IntroIndex { get; set; }
    public string OverrideVideoStreamId { get; set; } = string.Empty;
    public int ClientCount => ClientStreamerConfigurations.Keys.Count;

    public int StreamGroupId { get; set; }
    public int StreamGroupProfileId { get; set; }
    public CommandProfileDto CommandProfile { get; set; }
    public CustomPlayList? CustomPlayList { get; set; }
    public bool PlayedIntro { get; set; }

    public void SetSourceChannel(ChannelReader<byte[]> sourceChannelReader, CancellationToken cancellationToken)
    {
        Channel<byte[]> newChannel = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });

        // Stop the current channel processing
        _currentChannel.Writer.TryComplete();

        // Swap the channel
        _currentChannel = newChannel;

        // Start processing the new source channel
        StartProcessingSourceChannel(sourceChannelReader);
    }

    private void StartProcessingSourceChannel(ChannelReader<byte[]> sourceChannelReader)
    {
        CancellationToken token = _cancellationTokenSource.Token;

        _ = Task.Run(async () =>
        {
            await foreach (byte[] item in sourceChannelReader.ReadAllAsync(token))
            {
                await _currentChannel.Writer.WriteAsync(item, token).ConfigureAwait(false);
            }
        }, token);

        _ = Task.Run(async () =>
        {
            await foreach (byte[] item in _currentChannel.Reader.ReadAllAsync(token))
            {
                foreach (ISMStreamChannelBase<byte[]>? clientChannel in ClientStreamerConfigurations.Values.Where(a => a.ClientStream?.Channel != null).Select(a => a.ClientStream.Channel))
                {
                    await clientChannel.WriteAsync(item, token).ConfigureAwait(false);
                }
            }
        }, token);
    }

    public void SetCurrentSMStream(SMStreamDto? smStream)
    {
        if (smStream != null)
        {
            SMStream = smStream;
        }
    }

    public void SetIsGlobal()
    {
        IsGlobal = true;
    }
}