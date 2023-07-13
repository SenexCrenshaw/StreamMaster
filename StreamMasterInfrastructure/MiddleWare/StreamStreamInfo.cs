using StreamMasterApplication.Common.Models;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.MiddleWare;

public class StreamInformation : IDisposable
{
    private ConcurrentDictionary<Guid, StreamerConfiguration> _clientInformations;

    public StreamInformation(string streamUrl, CircularRingBuffer buffer, Task streamingTask, int m3uFileId, int maxStreams, int processId, CancellationTokenSource cancellationTokenSource)
    {
        StreamUrl = streamUrl;
        StreamerCancellationToken = cancellationTokenSource;
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

    public CircularRingBuffer RingBuffer { get; set; }

    public CancellationTokenSource StreamerCancellationToken { get; set; }

    public Task StreamingTask { get; set; }

    public string StreamUrl { get; set; }

    public bool AddStreamConfiguration(StreamerConfiguration streamerConfiguration)
    {
        return _clientInformations.TryAdd(streamerConfiguration.ClientId, streamerConfiguration);
    }

    public void Dispose()
    {
        Stop();
    }

    public StreamerConfiguration? GetStreamConfiguration(Guid ClientId)
    {
        return _clientInformations.FirstOrDefault(a => a.Value.ClientId == ClientId).Value;
    }

    public List<StreamerConfiguration> GetStreamConfigurations()
    {
        return _clientInformations.Values.ToList();
    }

    public bool MoveStreamConfiguration(StreamerConfiguration streamerConfiguration)
    {
        return _clientInformations.TryAdd(streamerConfiguration.ClientId, streamerConfiguration);
    }

    public bool RemoveStreamConfiguration(StreamerConfiguration streamerConfiguration)
    {
        return _clientInformations.TryRemove(streamerConfiguration.ClientId, out _);
    }

    public void Stop()
    {
        if (StreamerCancellationToken is not null && !StreamerCancellationToken.IsCancellationRequested)
        {
            StreamerCancellationToken.Cancel();
        }
    }

    internal void SetClientBufferDelegate(Guid ClientId, Func<CircularRingBuffer> func)
    {
        var sc = GetStreamConfiguration(ClientId);
        if (sc == null)
        {
            return;
        }
        sc.ReadBuffer.SetBufferDelegate(func);
    }
}
