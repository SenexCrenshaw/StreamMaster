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

    private readonly int GetVideoInfoCount = 0;
    private int GetVideoInfoErrors = 0;
    private readonly SemaphoreSlim buildVideoInfoSemaphore = new(1, 1);

    public VideoInfo GetVideoInfo()
    {
        return _videoInfo ?? new VideoInfo();
    }
    public async Task BuildVideoInfoAsync(byte[] videoMemory, CancellationToken cancellationToken = default)
    {
        bool isLocked = false;
        try
        {
            isLocked = await buildVideoInfoSemaphore.WaitAsync(0, cancellationToken).ConfigureAwait(false);
            if (!isLocked || GetVideoInfoErrors > 3)
            {
                return;
            }

            string ffprobeExec = GetFFProbeExecutablePath(settings);
            if (string.IsNullOrEmpty(ffprobeExec))
            {
                logger.LogError("FFProbe executable not found.");
                GetVideoInfoErrors = int.MaxValue;
                return;
            }

            try
            {
                VideoInfo? videoInfo = await CreateFFProbeStreamAsync(ffprobeExec, videoMemory, cancellationToken).ConfigureAwait(false);
                if (videoInfo == null || !videoInfo.IsValid())
                {
                    GetVideoInfoErrors++;
                    logger.LogError("Failed to deserialize FFProbe output.");
                    return;
                }

                _videoInfo = videoInfo;
                GetVideoInfoErrors = 0; // Reset errors on successful info retrieval
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
        if (!File.Exists(ffprobeExec) && !File.Exists(ffprobeExec + ".exe") && !IsFFProbeAvailable())
        {
            return string.Empty;
        }

        return File.Exists(ffprobeExec) ? ffprobeExec : "ffprobe";
    }

    private async Task<VideoInfo?> CreateFFProbeStreamAsync(string ffProbeExec, byte[] videoMemory, CancellationToken cancellationToken)
    {
        using var process = new Process();
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = ffProbeExec,
                Arguments = "-loglevel error -print_format json -show_format -sexagesimal -show_streams -",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };
            process.StartInfo = startInfo;

            bool processStarted = process.Start();
            if (!processStarted)
            {
                logger.LogError("CreateFFProbeStream Error: Failed to start FFProbe process");
                return null;
            }

            using var stdin = process.StandardInput.BaseStream;
            await stdin.WriteAsync(videoMemory, cancellationToken).ConfigureAwait(false);
            await stdin.FlushAsync(cancellationToken).ConfigureAwait(false);

            if (!process.WaitForExit(5000))
            {
                logger.LogWarning("FFProbe process did not exit within the expected time.");
                process.Kill();
            }

            string output = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<VideoInfo>(output);
        }
        catch (IOException ex)
        {
            logger.LogError(ex, "CreateFFProbeStream Error: IO Exception occurred");
            process.Kill();
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "CreateFFProbeStream Error: Failed to deserialize FFProbe output");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CreateFFProbeStream Error: Unexpected exception");
            process.Kill();
        }

        return null;
    }

    private static bool IsFFProbeAvailable()
    {
        string command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";
        var startInfo = new ProcessStartInfo(command, "ffprobe")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        process.WaitForExit();
        return process.ExitCode == 0;
    }
}