
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Cache;

using System.Collections.Concurrent;

namespace StreamMaster.Infrastructure.VideoStreamManager.Buffers;


/// <summary>
/// Represents a circular ring buffer for streaming data.
/// </summary>
public sealed partial class CircularRingBuffer : ICircularRingBuffer
{
    private const int maxDynamicWaitTimeMs = 100;
    private const int maxDataWaitTimeMs = 20;

    public event EventHandler<Guid> DataAvailable;

    private readonly ConcurrentDictionary<Guid, PerformanceBpsMetrics> _performanceMetrics = new();
    private readonly ConcurrentDictionary<Guid, int> _clientLastReadBeforeOverwrite = new();
    private readonly PerformanceBpsMetrics _writeMetric = new();

    private readonly IInputStatisticsManager _inputStatisticsManager;
    private readonly IInputStreamingStatistics _inputStreamStatistics;

    public readonly StreamInfo StreamInfo;

    private readonly Memory<byte> _buffer;
    private readonly int _bufferSize;

    public VideoInfo? VideoInfo { get; set; } = null;

    //private int _oldestDataIndex;
    private readonly float _preBuffPercent;
    private int _writeIndex { get; set; } = 0;
    private long WriteBytes { get; set; } = 0;

    private readonly ILogger<ICircularRingBuffer> logger;
    private readonly ILogger<ReadsLogger> _readLogger;
    private readonly ILogger<WriteLogger> _writeLogger;
    private readonly ILogger<OverWLogger> _overwriteLogger;
    private readonly ILogger<WaitsLogger> _waitLogger;
    private readonly ILogger<Dist_Logger> _distanceLogger;
    private readonly ILogger<StatsLogger> _statsLogger;


    private bool _disposed = false;
    private readonly bool HasBufferFlipped;
    public string VideoStreamName => StreamInfo.VideoStreamName;
    public Guid Id { get; } = Guid.NewGuid();
    public int BufferSize => _buffer.Length;
    public string VideoStreamId => StreamInfo.VideoStreamId;
    private bool InternalIsPreBuffered { get; set; } = false;


    public CancellationTokenSource StopVideoStreamingToken { get; set; }

    public CircularRingBuffer(VideoStreamDto videoStreamDto, string channelId, string channelName, IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, int rank, ILoggerFactory loggerFactory)
    {
        PauseReaders();
        Setting setting = memoryCache.GetSetting();
        _inputStatisticsManager = inputStatisticsManager ?? throw new ArgumentNullException(nameof(inputStatisticsManager));

        logger = loggerFactory.CreateLogger<CircularRingBuffer>();
        _readLogger = loggerFactory.CreateLogger<ReadsLogger>();
        _writeLogger = loggerFactory.CreateLogger<WriteLogger>();
        _overwriteLogger = loggerFactory.CreateLogger<OverWLogger>();
        _waitLogger = loggerFactory.CreateLogger<WaitsLogger>();
        _distanceLogger = loggerFactory.CreateLogger<Dist_Logger>();
        _statsLogger = loggerFactory.CreateLogger<StatsLogger>();
        if (setting.PreloadPercentage is < 0 or > 100)
        {
            setting.PreloadPercentage = 0;
        }

        if (setting.RingBufferSizeMB is < 1 or > 256)
        {
            setting.RingBufferSizeMB = 4;
        }

        _bufferSize = setting.RingBufferSizeMB * 1024 * 1000;
        _preBuffPercent = setting.PreloadPercentage;

        StreamInfo = new StreamInfo
        {
            ChannelId = channelId,
            ChannelName = channelName,
            VideoStreamId = videoStreamDto.Id,
            VideoStreamName = videoStreamDto.User_Tvg_name,
            Logo = videoStreamDto.User_Tvg_logo,
            StreamingProxyType = videoStreamDto.StreamingProxyType,
            StreamUrl = videoStreamDto.User_Url,

            Rank = rank
        };

        _inputStreamStatistics = _inputStatisticsManager.RegisterInputReader(StreamInfo);

        _buffer = new byte[_bufferSize];

        _writeIndex = 0;

        logger.LogInformation("New Circular Buffer {Id} for stream {videoStreamId} {name}", Id, videoStreamDto.Id, videoStreamDto.User_Tvg_name);
        //UnPauseReaders();
    }


    private void DoDispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _writeSignal.TrySetCanceled();
                _waitTime.GetAllLabelValues().ToList().ForEach(x => _waitTime.RemoveLabelled(x[0], x[1], x[2]));
                _bitsPerSecond.RemoveLabelled(Id.ToString(), StreamInfo.VideoStreamName);
                _bytesWrittenCounter.RemoveLabelled(Id.ToString(), StreamInfo.VideoStreamName);
                _writeErrorsCounter.RemoveLabelled(Id.ToString(), StreamInfo.VideoStreamName);
                _dataArrival.RemoveLabelled(Id.ToString(), StreamInfo.VideoStreamName);
                _clientLastReadBeforeOverwrite.Clear();
                _performanceMetrics.Clear();
            }

            // Dispose unmanaged resources here if any

            _disposed = true;
        }
    }

    // Public implementation of Dispose pattern callable by consumers
    public void Dispose()
    {
        DoDispose(true);
        GC.SuppressFinalize(this);
    }

    // Finalizer in case Dispose wasn't called
    ~CircularRingBuffer()
    {
        DoDispose(false);
    }

}
