namespace StreamMaster.Application.StreamGroups.QueriesOld;

public class StreamingStatusDto : StreamingClientInformation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public StreamingServiceStatusDto StreamingServiceStatus { get; set; } = new();

    public List<StreamingWorkerStatusDto> StreamingWorkerStatuses { get; set; } = [];
}

public class StreamingWorkerStatusDto : StreamingClientInformation
{
    public bool BufferIsReady { get; set; }
    public int BytesInBuffer { get; set; }
    public string? ChannelName { get; set; }
    public double ClientBps { get; set; }
    public int ClientBufferPosition { get; set; }
    public Guid ClientId { get; set; }
    public string? IconSource { get; set; }
    public string? ClientUserAgent { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();

    public double InboundBps { get; set; }
    public string? ProxiedUrl { get; set; }
    public int RingBufferSize { get; set; }
    public string? StreamingUrl { get; set; }
    public string? StreamName { get; set; }
    public string StreamProxyType { get; set; }
    public Guid StreamWorkerId { get; set; }
}
