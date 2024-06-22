using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Extensions;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace StreamMaster.Streams.Streams;


/// <summary>
/// Manages the streaming of a single video stream, including client registrations and circularRingbuffer handling.
/// </summary>
public sealed partial class StreamHandler
{
    private readonly CancellationTokenSource cancellationTokenSource = new();

    private readonly SemaphoreSlim buildVideoInfoSemaphore = new(1, 1);
    private readonly FileSaver fileSaver = new(5);

    public VideoInfo GetVideoInfo()
    {
        return _videoInfo ?? new();
    }

    public async Task BuildVideoInfoAsync(byte[] videoMemory)
    {
        // string testDir = Path.Combine(BuildInfo.AppDataFolder, "test.mp4");
        //await fileSaver.SaveVideoWithRevisionsAsync(videoMemory, testDir);
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

            string ffprobeExec = GetFFProbeExecutablePath(settings);

            if (string.IsNullOrEmpty(ffprobeExec))
            {
                logger.LogError("FFProbe executable not found.");
                GetVideoInfoErrors = int.MaxValue;
                return;
            }

            VideoInfo? videoInfo = await CreateFFProbeStream(ffprobeExec, videoMemory).ConfigureAwait(false);

            if (!videoInfo.IsValid())
            {
                GetVideoInfoErrors++;
                logger.LogError("Failed to deserialize FFProbe output.");
                return;
            }

            LastVideoInfoRun = SMDT.UtcNow;
            _videoInfo = videoInfo;
            logger.LogInformation("Retrieved video information for {VideoStreamName}.", SMStream.Name);
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
        finally
        {
            if (isLocked)
            {
                buildVideoInfoSemaphore.Release();
            }
        }
    }



    private string GetFFProbeExecutablePath(Setting settings)
    {
        string ffprobeExec = Path.Combine(BuildInfo.AppDataFolder, settings.FFProbeExecutable);
        return File.Exists(ffprobeExec) || File.Exists(ffprobeExec + ".exe") || IsFFProbeAvailable()
            ? ffprobeExec
            : string.Empty;
    }

    private readonly int GetVideoInfoCount = 0;
    private int GetVideoInfoErrors = 0;
    private async Task<VideoInfo?> CreateFFProbeStream(string ffProbeExec, byte[] videoMemory)
    {
        using Process process = new();
        try
        {
            string options = "-loglevel error -print_format json -show_format -sexagesimal -show_streams - ";
            process.StartInfo.FileName = ffProbeExec;
            process.StartInfo.Arguments = options;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;

            if (!process.Start())
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
                logger.LogWarning("Process did not exit within the expected time.");
            }

            string output = await process.StandardOutput.ReadToEndAsync();
            VideoInfo? videoInfo = JsonSerializer.Deserialize<VideoInfo>(output);

            if (videoInfo == null)
            {
                var jsonString = JsonSerializer.Serialize(videoInfo);
                logger.LogError("CreateFFProbeStream Error: Failed to deserialize FFProbe output");
                logger.LogDebug(jsonString);
                return null;
            }
            _videoInfo = videoInfo;
            return videoInfo;
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            logger.LogError(ex, "CreateFFProbeStream Error: {ErrorMessage}");
            process.Kill();
            return null;
        }
    }

    private static bool IsFFProbeAvailable()
    {
        string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";
        ProcessStartInfo startInfo = new(command, "ffprobe")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        using Process process = new()
        {
            StartInfo = startInfo
        };
        process.Start();
        process.WaitForExit();
        return process.ExitCode == 0;
    }
}