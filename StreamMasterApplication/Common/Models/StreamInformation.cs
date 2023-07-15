using System.Collections.Concurrent;

namespace StreamMasterApplication.Common.Models;

public class StreamInformation : IDisposable
{
    private ConcurrentDictionary<Guid, ClientStreamerConfiguration> _clientInformations;

    //public StreamInformation()
    //{
    //    M3UStream = false;
    //    _clientInformations = new();
    //}

    public StreamInformation(string streamUrl, ICircularRingBuffer buffer, Task streamingTask, int m3uFileId, int maxStreams, int processId, CancellationTokenSource cancellationTokenSource)
    {
        StreamUrl = streamUrl;
        VideoStreamingCancellationToken = cancellationTokenSource;
        StreamingTask = streamingTask;
        RingBuffer = buffer;
        M3UFileId = m3uFileId;
        MaxStreams = maxStreams;
        ProcessId = processId;
        M3UStream = false;
        _clientInformations = new();
    }

    public int ClientCount => _clientInformations.Count;
    public bool FailoverInProgress { get; set; }

    public int M3UFileId { get; set; }

    public bool M3UStream { get; set; }

    public int MaxStreams { get; set; }

    public int ProcessId { get; set; } = -1;

    public ICircularRingBuffer RingBuffer { get; set; }

    public Task StreamingTask { get; set; }
    public string StreamUrl { get; set; }
    public CancellationTokenSource VideoStreamingCancellationToken { get; set; }

    public bool AddStreamConfiguration(ClientStreamerConfiguration streamerConfiguration)
    {
        return _clientInformations.TryAdd(streamerConfiguration.ClientId, streamerConfiguration);
    }

    public void Dispose()
    {
        Stop();
    }

    public ClientStreamerConfiguration? GetStreamConfiguration(Guid ClientId)
    {
        return _clientInformations.FirstOrDefault(a => a.Value.ClientId == ClientId).Value;
    }

    public List<ClientStreamerConfiguration> GetStreamConfigurations()
    {
        return _clientInformations.Values.ToList();
    }

    public bool MoveStreamConfiguration(ClientStreamerConfiguration streamerConfiguration)
    {
        return _clientInformations.TryAdd(streamerConfiguration.ClientId, streamerConfiguration);
    }

    public bool RemoveStreamConfiguration(ClientStreamerConfiguration streamerConfiguration)
    {
        return _clientInformations.TryRemove(streamerConfiguration.ClientId, out _);
    }

    public void SetClientBufferDelegate(Guid ClientId, Func<ICircularRingBuffer> func)
    {
        var sc = GetStreamConfiguration(ClientId);
        if (sc == null)
        {
            return;
        }

        sc.ReadBuffer.SetBufferDelegate(func, sc);
    }

    public void Stop()
    {
        if (VideoStreamingCancellationToken is not null && !VideoStreamingCancellationToken.IsCancellationRequested)
        {
            VideoStreamingCancellationToken.Cancel();
        }
    }
}
