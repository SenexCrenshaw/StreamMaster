using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using System.Collections.Concurrent;
using System.Diagnostics;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class StreamInformation : IDisposable, IStreamInformation
{
    private readonly ConcurrentDictionary<Guid, ClientStreamerConfiguration> _clientInformations;

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

    public ICircularRingBuffer RingBuffer { get; private set; }

    public Task StreamingTask { get; set; }
    public string StreamUrl { get; set; }
    public CancellationTokenSource VideoStreamingCancellationToken { get; set; }

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

    public void RegisterStreamConfiguration(ClientStreamerConfiguration streamerConfiguration)
    {
        _clientInformations.TryAdd(streamerConfiguration.ClientId, streamerConfiguration);
        RingBuffer.RegisterClient(streamerConfiguration.ClientId, streamerConfiguration.ClientUserAgent);

        SetClientBufferDelegate(streamerConfiguration, () => RingBuffer);
    }

    public void Stop()
    {
        if (VideoStreamingCancellationToken is not null && !VideoStreamingCancellationToken.IsCancellationRequested)
        {
            VideoStreamingCancellationToken.Cancel();
        }

        if (ProcessId > 0)
        {
            try
            {
                string? procName = CheckProcessExists(ProcessId);
                if (procName != null)
                {
                    Process process = Process.GetProcessById(ProcessId);
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error killing process {ProcessId} : {ex}", ProcessId, ex.ToString());
            }
        }
    }

    public bool UnRegisterStreamConfiguration(ClientStreamerConfiguration streamerConfiguration)
    {
        _clientInformations.TryAdd(streamerConfiguration.ClientId, streamerConfiguration);
        RingBuffer.UnregisterClient(streamerConfiguration.ClientId);

        return _clientInformations.TryRemove(streamerConfiguration.ClientId, out _);
    }

    private static string? CheckProcessExists(int processId)
    {
        try
        {
            Process process = Process.GetProcessById(processId);
            Console.WriteLine($"Process with ID {processId} exists. Name: {process.ProcessName}");
            return process.ProcessName;
        }
        catch (ArgumentException)
        {
            Console.WriteLine($"Process with ID {processId} does not exist.");
            return null;
        }
    }

    private void SetClientBufferDelegate(ClientStreamerConfiguration config, Func<ICircularRingBuffer> func)
    {
        ClientStreamerConfiguration? sc = GetStreamConfiguration(config.ClientId);
        if (sc is null || sc.ReadBuffer is null)
        {
            return;
        }

        sc.ReadBuffer.SetBufferDelegate(func, sc);
    }
}
