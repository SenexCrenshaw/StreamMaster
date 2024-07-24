using StreamMaster.Domain.Configuration;

using System.Threading.Channels;

namespace StreamMaster.Streams.Streams;

public sealed partial class StreamHandler : IStreamHandler, IDisposable
{
    public event EventHandler<StreamHandlerStopped> OnStreamingStoppedEvent;
    public ChannelReader<byte[]> GetOutputChannelReader()
    {
        return _outputChannel.Reader;
    }
    private readonly ILogger<IStreamHandler> logger;
    public SMStreamDto SMStream { get; }
    private int ProcessId { get; set; }
    public IOptionsMonitor<Setting> intSettings;
    private VideoInfo? _videoInfo = null;
    private CancellationTokenSource VideoStreamingCancellationToken { get; set; } = new();
    private readonly ILoggerFactory loggerFactory;
    public bool IsFailed { get; set; }

    public readonly StreamInfo StreamInfo;

    public StreamHandler(SMStreamDto SMStream, int processId, IOptionsMonitor<Setting> intSettings, ILoggerFactory loggerFactory)
    {
        this.intSettings = intSettings;
        logger = loggerFactory.CreateLogger<StreamHandler>();
        this.loggerFactory = loggerFactory;
        this.SMStream = SMStream;
        ProcessId = processId;
    }

    private readonly Channel<byte[]> _outputChannel = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });

    private void OnStreamingStopped(bool InputStreamError)
    {
        OnStreamingStoppedEvent?.Invoke(this, new StreamHandlerStopped { StreamUrl = SMStream.Url, InputStreamError = InputStreamError });
    }

    public void CancelStreamThread()
    {
        VideoStreamingCancellationToken?.Cancel();
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public void Stop(bool inputStreamError = false)
    {
        SetFailed();

        if (VideoStreamingCancellationToken?.IsCancellationRequested == false)
        {
            VideoStreamingCancellationToken.Cancel();
        }

        if (ProcessId > 0)
        {

            try
            {
                _ = ProcessHelper.KillProcessById(ProcessId);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error killing process {ProcessId}.", ProcessId);
            }
        }
    }

    public void SetFailed()
    {
        IsFailed = true;
    }
}