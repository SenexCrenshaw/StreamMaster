using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace StreamMasterInfrastructure.VideoStreamManager.Streams;


/// <summary>
/// Manages the streaming of a single video stream, including client registrations and circularRingbuffer handling.
/// </summary>
public sealed class StreamHandler(VideoStreamDto videoStreamDto, int processId, IMemoryCache memoryCache, ILogger<IStreamHandler> logger, ICircularRingBuffer ringBuffer) : IStreamHandler
{
    private readonly Memory<byte> startMemory = new byte[1 * 1024 * 1024];
    private int startMemoryIndex = 0;
    private bool startMemoryFilled = false;

    public event EventHandler<string> OnStreamingStoppedEvent;

    private readonly ConcurrentDictionary<Guid, Guid> clientStreamerIds = new();

    public int M3UFileId { get; } = videoStreamDto.M3UFileId;
    public int ProcessId { get; set; } = processId;
    public ICircularRingBuffer CircularRingBuffer { get; } = ringBuffer;
    public string StreamUrl { get; } = videoStreamDto.User_Url;
    public string VideoStreamId { get; } = videoStreamDto.Id;
    public string VideoStreamName { get; } = videoStreamDto.User_Tvg_name;

    private VideoInfo? _videoInfo = null;
    private CancellationTokenSource VideoStreamingCancellationToken { get; set; } = new();

    public int ClientCount => clientStreamerIds.Count;

    public bool IsFailed { get; private set; }

    private void OnStreamingStopped()
    {
        OnStreamingStoppedEvent?.Invoke(this, StreamUrl);
    }

    public async Task<VideoInfo> GetVideoInfo()
    {
        if (_videoInfo != null)
        {
            return _videoInfo;
        }

        if (!startMemoryFilled)
        {
            logger.LogWarning("Can not get video information, not enough data");

            return new();
        }
        Setting settings = memoryCache.GetSetting();

        string ffprobeExec = Path.Combine(BuildInfo.AppDataFolder, settings.FFProbeExecutable);

        if (!File.Exists(ffprobeExec) && !File.Exists(ffprobeExec + ".exe"))
        {
            if (!IsFFProbeAvailable())
            {
                return new();
            }
            ffprobeExec = "ffprobe";
        }

        try
        {

            return await CreateFFProbeStream(ffprobeExec).ConfigureAwait(false);
        }
        catch (IOException ex)
        {

        }
        catch (Exception ex)
        {

        }
        return new();
    }

    private async Task<VideoInfo> CreateFFProbeStream(string ffProbeExec)
    {
        using Process process = new();
        try
        {
            Setting settings = memoryCache.GetSetting();

            string options = "-loglevel error -print_format json -show_format -sexagesimal -show_streams - ";



            process.StartInfo.FileName = ffProbeExec;
            process.StartInfo.Arguments = options;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;

            bool processStarted = process.Start();
            if (!processStarted)
            {
                logger.LogError("CreateFFProbeStream Error: Failed to start FFProbe process");
                return new();
            }

            //Memory<byte> inputData = CircularRingBuffer.GetBufferSlice(1 * 1024 * 1024);
            byte[] buffer = startMemory.ToArray(); // Convert Memory<byte> to byte array

            //using CancellationTokenSource cts = new();
            //cts.CancelAfter(TimeSpan.FromSeconds(5)); // Set your timeout duration here

            using Timer timer = new(delegate { process.Kill(); }, null, 5000, Timeout.Infinite);


            using (Stream stdin = process.StandardInput.BaseStream)
            {
                await stdin.WriteAsync(buffer);
                await stdin.FlushAsync();
            }

            if (!process.WaitForExit(5000)) // 5000 ms timeout
            {
                // Handle the case where process doesn't exit in time
                logger.LogWarning("Process did not exit within the expected time.");
            }

            // Reading from the process's standard output
            string output = await process.StandardOutput.ReadToEndAsync();

            VideoInfo? videoInfo = JsonSerializer.Deserialize<VideoInfo>(output);
            if (videoInfo == null)
            {
                logger.LogError("CreateFFProbeStream Error: Failed to deserialize FFProbe output");
                return new();
            }
            _videoInfo = videoInfo;
            return videoInfo;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CreateFFProbeStream Error: {ErrorMessage}", ex.Message);
            process.Kill();
        }
        return new();
    }

    private static bool IsFFProbeAvailable()
    {
        string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";
        ProcessStartInfo startInfo = new(command, "ffprobe")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        Process process = new()
        {
            StartInfo = startInfo
        };
        _ = process.Start();
        process.WaitForExit();
        return process.ExitCode == 0;
    }

    //private async Task DelayWithCancellation(int milliseconds)
    //{
    //    try
    //    {
    //        await Task.Delay(milliseconds, VideoStreamingCancellationToken.Token);
    //    }
    //    catch (TaskCanceledException)
    //    {
    //        logger.LogInformation("Task was cancelled");
    //        throw;
    //    }
    //}

    //private async Task LogRetryAndDelay(int retryCount, int maxRetries, int waitTime)
    //{
    //    if (VideoStreamingCancellationToken.Token.IsCancellationRequested)
    //    {
    //        logger.LogInformation("Stream was cancelled for: {StreamUrl} {name}", StreamUrl, VideoStreamName);
    //    }

    //    logger.LogInformation("Stream received 0 bytes for stream: {StreamUrl} Retry {retryCount}/{maxRetries} {name}",
    //        StreamUrl,
    //        retryCount,
    //        maxRetries, VideoStreamName);

    //    await DelayWithCancellation(waitTime);
    //}

    public async Task StartVideoStreamingAsync(Stream stream)
    {
        const int chunkSize = 64 * 1024;

        logger.LogInformation("Starting video read streaming, chunk size is {ChunkSize}, for stream: {StreamUrl} name: {name} circularRingbuffer id: {circularRingbuffer}", chunkSize, StreamUrl, VideoStreamName, CircularRingBuffer.Id);

        Memory<byte> bufferMemory = new byte[chunkSize];

        //const int maxRetries = 3;
        //const int waitTime = 50;

        using (stream)
        {
            while (!VideoStreamingCancellationToken.IsCancellationRequested)// && retryCount < maxRetries)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(bufferMemory);
                    if (bytesRead < 1)
                    {
                        break;
                    }
                    else
                    {
                        if (!startMemoryFilled)
                        {
                            if (startMemoryIndex < startMemory.Length)
                            {
                                // Calculate the maximum number of bytes that can be copied
                                int bytesToCopy = Math.Min(startMemory.Length - startMemoryIndex, bytesRead);

                                // Directly copy the data from bufferMemory to startMemory
                                bufferMemory[..bytesToCopy].CopyTo(startMemory[startMemoryIndex..]);

                                // Update startMemoryIndex
                                startMemoryIndex += bytesToCopy;
                            }
                            else
                            {
                                startMemoryFilled = true;
                            }
                        }

                        CircularRingBuffer.WriteChunk(bufferMemory[..bytesRead]);
                    }
                }
                catch (TaskCanceledException)
                {
                    logger.LogInformation("Stream requested to stop for: {StreamUrl} {name}", StreamUrl, VideoStreamName);
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Stream error for: {StreamUrl} {name}", StreamUrl, VideoStreamName);
                    break;
                }
            }
        }

        stream.Close();
        stream.Dispose();

        //logger.LogInformation("Stream stopped for: {StreamUrl} {name}", StreamUrl, VideoStreamName);

        OnStreamingStopped();
    }



    public void Dispose()
    {
        clientStreamerIds.Clear();
        Stop();
        GC.SuppressFinalize(this);
    }

    public void Stop()
    {
        if (VideoStreamingCancellationToken?.IsCancellationRequested == false)
        {
            VideoStreamingCancellationToken.Cancel();
        }

        if (ProcessId > 0)
        {
            try
            {
                string? procName = CheckProcessExists();
                if (procName != null)
                {
                    Process process = Process.GetProcessById(ProcessId);
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error killing process {ProcessId}.", ProcessId);
            }
        }
    }

    public void RegisterClientStreamer(IClientStreamerConfiguration streamerConfiguration)
    {
        try
        {
            clientStreamerIds.TryAdd(streamerConfiguration.ClientId, streamerConfiguration.ClientId);
            CircularRingBuffer.RegisterClient(streamerConfiguration);

            logger.LogInformation("RegisterClientStreamer for Client ID {ClientId} to Video Stream Id {videoStreamId} {name} {RingBuffer.Id}", streamerConfiguration.ClientId, VideoStreamId, VideoStreamName, CircularRingBuffer.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering stream configuration for client {ClientId} {name} {RingBuffer.Id}", streamerConfiguration.ClientId, VideoStreamName, CircularRingBuffer.Id);
        }
    }


    public bool UnRegisterClientStreamer(Guid ClientId)
    {
        try
        {
            logger.LogInformation("UnRegisterClientStreamer ClientId: {ClientId} {name} {RingBuffer.Id}", ClientId, VideoStreamName, CircularRingBuffer.Id);
            bool result = clientStreamerIds.TryRemove(ClientId, out _);
            CircularRingBuffer.UnRegisterClient(ClientId);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error unregistering stream configuration for client {ClientId} {name} {RingBuffer.Id}", ClientId, VideoStreamName, CircularRingBuffer.Id);
            return false;
        }
    }

    private string? CheckProcessExists()
    {
        try
        {
            Process process = Process.GetProcessById(ProcessId);
            // logger.LogInformation($"Process with ID {processId} exists. Name: {process.ProcessName}");
            return process.ProcessName;
        }
        catch (ArgumentException)
        {
            // logger.LogWarning($"Process with ID {processId} does not exist.");
            return null;
        }
    }


    public IEnumerable<Guid> GetClientStreamerClientIds()
    {
        return clientStreamerIds.Keys;
    }


    public void SetFailed()
    {
        IsFailed = true;
    }

    public bool HasClient(Guid clientId)
    {
        return clientStreamerIds.ContainsKey(clientId);
    }
}