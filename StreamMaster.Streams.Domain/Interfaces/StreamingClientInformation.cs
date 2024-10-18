namespace StreamMaster.Streams.Domain.Interfaces;

public class M3UStreamHealth
{
    public DateTime LastError { get; set; }
    //public int M3UStreamId { get; set; }
    public int TotalErros { get; set; }
}

public class StreamingClientInformation
{
    public StreamingClientInformation()
    {
        StartDateTime = DateTime.Now;
    }

    public int ClientErrors { get; set; }
    public string Host { get; set; } = string.Empty;

    public List<M3UStreamHealth> M3UStreamHealths { get; set; } = [];
    public string RemoteIP { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public string UserAgent { get; set; } = string.Empty;
}

public class StreamingClientConfiguration : StreamingClientInformation
{
    public int BufferPosition { get; set; } = 0;
    public CancellationToken CancellationToken { get; set; }
    public int CurrentM3UStreamId { get; set; }
    public string CurrentM3UStreamName { get; set; } = string.Empty;
    public string CurrentM3UStreamProxyType { get; set; }
    public string CurrentM3UStreamUrl { get; set; } = string.Empty;
    public Guid Id { get; set; } = Guid.NewGuid();
    public bool IsReader { get; set; }
    public double LastBps { get; set; }
    public int LastBufferPosition { get; set; } = 0;

    // public int LastMemoryBufferPosition { get; set; } = 0;
    public Stream? OutputStream { get; set; }

    public bool StreamBufferLooped { get; set; } = false;

    public SMStreamDto? SMStream { get; set; }

    //public M3UStreamDto M3UStream { get; set; }
    public List<SMStreamDto> SMStreams { get; set; } = [];
}
