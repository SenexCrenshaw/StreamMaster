namespace StreamMasterInfrastructure.MiddleWare;

public class StreamStreamInfo : IDisposable
{
    public StreamStreamInfo(string streamUrl, CircularRingBuffer buffer, Task streamingTask, int m3uFileId, int maxStreams,int processId,CancellationTokenSource cancellationTokenSource)
    {
        StreamUrl = streamUrl;
        StreamerCancellationToken = cancellationTokenSource;
        StreamingTask = streamingTask;
        RingBuffer = buffer;
        M3UFileId =  m3uFileId;
        MaxStreams = maxStreams;
        ProcessId = processId;
}
    public int ProcessId { get; set; } = -1;
    public int M3UFileId { get; set; }
    public int MaxStreams { get; set; }
    public int ClientCounter { get; set; }
    public bool FailoverInProgress { get; set; }
  
    public CircularRingBuffer RingBuffer { get; set; }

    public CancellationTokenSource StreamerCancellationToken { get; set; }

    public Task StreamingTask { get; set; }
    public string StreamUrl { get; set; }

    public void Dispose()
    {
        Stop();
    }

    public void Stop()
    {
        if (StreamerCancellationToken is not null && !StreamerCancellationToken.IsCancellationRequested)
        {
            StreamerCancellationToken.Cancel();
        }
    }
}
