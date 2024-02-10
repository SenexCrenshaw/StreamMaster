using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Cache;
using StreamMaster.Domain.Extensions;
using StreamMaster.Domain.Models;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Channels;

namespace StreamMaster.Streams.Streams;


/// <summary>
/// Manages the streaming of a single video stream, including client registrations and circularRingbuffer handling.
/// </summary>
public sealed partial class StreamHandler
{
    private readonly CancellationTokenSource cancellationTokenSource = new();

    private readonly SemaphoreSlim buildVideoInfoSemaphore = new(1, 1);


    public VideoInfo GetVideoInfo()
    {
        return _videoInfo ?? new();
    }


    private async Task FillVideoMemoryAsync(byte[] videoMemory, ChannelReader<Memory<byte>> videoChannelReader, CancellationToken cancellationToken)
    {
        int position = 0;

        try
        {
            while (await videoChannelReader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                while (videoChannelReader.TryRead(out Memory<byte> mem) && mem.Length > 0)
                {
                    if (position + mem.Length > videoMemory.Length)
                    {
                        mem = mem[..(videoMemory.Length - position)];
                    }

                    mem.Span.CopyTo(new Span<byte>(videoMemory, position, mem.Length));
                    position += mem.Length;

                    if (position >= videoMemory.Length)
                    {
                        return;
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Operation was cancelled.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while filling video memory.");
            throw;
        }
    }

    public async Task BuildVideoInfoAsync(byte[] videoMemory)
    {
        bool isLocked = false;
        try
        {
            // Try to enter the semaphore, skip execution if already entered
            isLocked = await buildVideoInfoSemaphore.WaitAsync(0).ConfigureAwait(false);
            if (!isLocked)
            {
                logger.LogWarning("BuildVideoInfo execution is skipped because another operation is already running.");
                return;
            }

            if (GetVideoInfoErrors > 3)
            {
                logger.LogWarning("Skipped BuildVideoInfo due to excessive errors.");
                return;
            }

            Setting settings = memoryCache.GetSetting();
            string ffprobeExec = GetFFProbeExecutablePath(settings);

            if (string.IsNullOrEmpty(ffprobeExec))
            {
                logger.LogError("FFProbe executable not found.");
                GetVideoInfoErrors = int.MaxValue;
                return;
            }

            try
            {

                VideoInfo? videoInfo = await CreateFFProbeStream(ffprobeExec, videoMemory).ConfigureAwait(false);

                if (!videoInfo.IsValid())
                {
                    GetVideoInfoErrors++;
                    logger.LogError("Failed to deserialize FFProbe output.");
                    return;
                }

                LastVideoInfoRun = SMDT.UtcNow;
                logger.LogInformation("Retrieved video information for {VideoStreamName}.", VideoStreamName);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("BuildVideoInfo operation was canceled.");
            }
            catch (Exception ex)
            {
                GetVideoInfoErrors++;
                logger.LogError(ex, "An unexpected error occurred while building video info.");
            }
        }
        finally
        {
            if (isLocked)
            {
                buildVideoInfoSemaphore.Release();
            }
        }
    }

    //private async Task<byte[]> PrepareVideoMemoryAsync()
    //{
    //    byte[] videoMemory = new byte[videoBufferSize];
    //    await FillVideoMemoryAsync(videoMemory, videoChannel.Reader, cancellationTokenSource.Token).ConfigureAwait(false);
    //    return videoMemory;
    //}

    private string GetFFProbeExecutablePath(Setting settings)
    {
        string ffprobeExec = Path.Combine(BuildInfo.AppDataFolder, settings.FFProbeExecutable);
        return !File.Exists(ffprobeExec) && !File.Exists(ffprobeExec + ".exe") && !IsFFProbeAvailable()
            ? string.Empty
            : File.Exists(ffprobeExec) ? ffprobeExec : "ffprobe";
    }

    private readonly int GetVideoInfoCount = 0;
    private int GetVideoInfoErrors = 0;
    private async Task<VideoInfo?> CreateFFProbeStream(string ffProbeExec, byte[] videoMemory)
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
                return null;
            }

            using Timer timer = new(delegate { process.Kill(); }, null, 5000, Timeout.Infinite);

            using (Stream stdin = process.StandardInput.BaseStream)
            {
                await stdin.WriteAsync(videoMemory);
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
                return null;
            }
            _videoInfo = videoInfo;
            return videoInfo;

        }
        catch (Exception ex) when (ex is IOException or JsonException or Exception)
        {
            //logger.LogError( "CreateFFProbeStream Error: {ErrorMessage}");
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
}