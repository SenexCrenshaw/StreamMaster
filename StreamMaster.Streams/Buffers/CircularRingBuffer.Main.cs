
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


    public event EventHandler<Guid> DataAvailable;
    private readonly IMemoryCache memoryCache;

    private readonly ConcurrentDictionary<Guid, PerformanceBpsMetrics> _performanceMetrics = new();
    private readonly ConcurrentDictionary<Guid, int> _clientLastReadBeforeOverwrite = new();
    private readonly PerformanceBpsMetrics _writeMetric = new();

    private readonly IInputStatisticsManager _inputStatisticsManager;
    private readonly IInputStreamingStatistics _inputStreamStatistics;

    public readonly StreamInfo StreamInfo;

    private Memory<byte> _buffer;
    private readonly int _bufferSize;

    public VideoInfo? VideoInfo { get; set; } = null;

    private int _writeIndex { get; set; } = 0;
    private long WriteBytes { get; set; } = 0;

    private readonly ILogger<ICircularRingBuffer> logger;
    private readonly ILogger<ReadsLogger> _readLogger;
    private readonly ILogger<WriteLogger> _writeLogger;
    private readonly ILogger<CircularBufferLogger> _circularBufferLogger;

    private bool _disposed = false;

    public string VideoStreamName => StreamInfo.VideoStreamName;
    public Guid Id { get; } = Guid.NewGuid();
    public int BufferSize => _buffer.Length;
    public string VideoStreamId => StreamInfo.VideoStreamId;



    public CancellationTokenSource StopVideoStreamingToken { get; set; }

    public CircularRingBuffer(VideoStreamDto videoStreamDto, string channelId, string channelName, IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, int rank, ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<CircularRingBuffer>();
        _readLogger = loggerFactory.CreateLogger<ReadsLogger>();
        _writeLogger = loggerFactory.CreateLogger<WriteLogger>();
        _circularBufferLogger = loggerFactory.CreateLogger<CircularBufferLogger>();
        this.memoryCache = memoryCache;

        PauseReaders();
        Setting setting = memoryCache.GetSetting();
        _inputStatisticsManager = inputStatisticsManager ?? throw new ArgumentNullException(nameof(inputStatisticsManager));


        //if (setting.PreloadPercentage is < 0 or > 100)
        //{
        //    setting.PreloadPercentage = 0;
        //}

        if (setting.RingBufferSizeMB is < 1 or > 256)
        {
            setting.RingBufferSizeMB = 4;
        }

        _bufferSize = setting.RingBufferSizeMB * 1024 * 1000;
        //_preBuffPercent = setting.PreloadPercentage;

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
                //_writeSignal.TrySetCanceled();
                //_waitTime.GetAllLabelValues().ToList().ForEach(x => _waitTime.RemoveLabelled(x[0], x[1], x[2]));
                //_waitTime.Unpublish();

                //_bitsPerSecond.RemoveLabelled(Id.ToString(), StreamInfo.VideoStreamName);
                //_bitsPerSecond.Unpublish();

                //_bytesWrittenCounter.RemoveLabelled(Id.ToString(), StreamInfo.VideoStreamName);
                //_bytesWrittenCounter.Unpublish();

                //_writeErrorsCounter.RemoveLabelled(Id.ToString(), StreamInfo.VideoStreamName);
                //_writeErrorsCounter.Unpublish();

                //_dataArrival.RemoveLabelled(Id.ToString(), StreamInfo.VideoStreamName);
                //_dataArrival.Unpublish();

                _bitsPerSecond.RemoveLabelled(StreamInfo.VideoStreamName);
                _bitsPerSecond.Unpublish();

                _bytesWrittenCounter.RemoveLabelled(StreamInfo.VideoStreamName);
                _bytesWrittenCounter.Unpublish();

                _writeErrorsCounter.RemoveLabelled(StreamInfo.VideoStreamName);
                _writeErrorsCounter.Unpublish();

                _dataArrival.RemoveLabelled(StreamInfo.VideoStreamName);
                _dataArrival.Unpublish();

                _clientLastReadBeforeOverwrite.Clear();
                _performanceMetrics.Clear();

                _buffer = null;
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
