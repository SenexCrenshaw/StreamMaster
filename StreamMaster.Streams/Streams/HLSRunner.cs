using StreamMaster.Application.Interfaces;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Crypto;
using StreamMaster.Streams.Domain.Args;

using System.Diagnostics;

namespace StreamMaster.Streams.Streams
{
    public class HLSRunner : IHLSRunner
    {
        public event EventHandler<ProcessExitEventArgs>? ProcessExited;

        private readonly ILogger<HLSRunner> logger;
        private readonly IOptionsMonitor<Setting> intSettings;
        private readonly IOptionsMonitor<HLSSettings> intHLSSettings;
        private readonly ICryptoService cryptoService;
        private Process? process;

        public HLSRunner(
            ILogger<HLSRunner> logger,
            IM3U8ChannelStatus channelStatus,
            ICryptoService cryptoService,
            IOptionsMonitor<Setting> intSettings,
            IOptionsMonitor<HLSSettings> intHLSSettings)
        {
            this.cryptoService = cryptoService;
            this.logger = logger;
            ChannelStatus = channelStatus;
            this.intSettings = intSettings;
            this.intHLSSettings = intHLSSettings;
        }

        public IM3U8ChannelStatus ChannelStatus { get; }
        public int ProcessId => process?.Id ?? -1;

        public async Task HLSStartStreamingInBackgroundAsync(CancellationToken cancellationToken)
        {
            try
            {
                Task<(int? processId, ProxyStreamError? error)> streamingTask = Task.Run(() => CreateHLSProcessAsync(ChannelStatus, cancellationToken), cancellationToken);

                await streamingTask.ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        logger.LogError("Streaming task failed: {Exception}", task.Exception?.GetBaseException().Message);
                    }
                    else if (task.IsCompletedSuccessfully)
                    {
                        logger.LogInformation("Streaming task completed successfully.");
                    }
                }, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in HLSStartStreamingInBackgroundAsync");
            }
        }

        private async Task<(int? processId, ProxyStreamError? error)> CreateHLSProcessAsync(IM3U8ChannelStatus channelStatus, CancellationToken cancellationToken)
        {
            Setting settings = intSettings.CurrentValue;
            HLSSettings hlssettings = intHLSSettings.CurrentValue;
            try
            {
                if (channelStatus.SMStreamInfo == null)
                {
                    ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = "Stream info not found" };
                    logger.LogError("CreateHLSProcessAsync Error: {ErrorMessage}", error.Message);
                    return (null, error);
                }

                string? encodedString = cryptoService.EncodeString(channelStatus.SMStreamInfo.Id);
                if (encodedString == null)
                {
                    ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = "Encode error" };
                    logger.LogError("Encode error");
                    return (null, error);
                }

                string? exec = FileUtil.GetExec(settings.FFMPegExecutable);
                if (exec == null)
                {
                    logger.LogCritical("Command {command} not found", settings.FFMPegExecutable);
                    return (null, new ProxyStreamError { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"Command {settings.FFMPegExecutable} not found" });
                }

                string outputdir = channelStatus.M3U8Directory;
                if (!outputdir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    outputdir += Path.DirectorySeparatorChar.ToString();
                }

                if (!Directory.Exists(outputdir))
                {
                    Directory.CreateDirectory(outputdir);
                }

                string args = hlssettings.HLSFFMPEGOptions;
                if (channelStatus.SMStreamInfo.Url.Contains("://"))
                {
                    string clientUserAgent = !string.IsNullOrEmpty(channelStatus.ClientUserAgent) ? channelStatus.ClientUserAgent : settings.SourceClientUserAgent;
                    args += $" -user_agent \"{clientUserAgent}\"";
                }

                args += " -ss 2";
                args = args.Replace("{streamUrl}", $"\"{channelStatus.SMStreamInfo.Url}\"").Trim();
                args += $" -reconnect_delay_max {hlssettings.HLSReconnectDurationInSeconds}";
                args += $" -hls_time {hlssettings.HLSSegmentDurationInSeconds}";
                args += $" -hls_list_size {hlssettings.HLSSegmentCount}";
                args += $" -hls_delete_threshold {hlssettings.HLSSegmentCount * 2}";
                args += $" -hls_base_url \"{encodedString}/\"";
                args += $" -hls_segment_filename \"{outputdir}%d.ts\"";
                args += $" \"{outputdir}index.m3u8\"";

                process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = exec,
                        Arguments = args,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    },
                    EnableRaisingEvents = true
                };

                process.ErrorDataReceived += Process_ErrorDataReceived;
                process.Exited += Process_Exited;

                bool processStarted = process.Start();
                process.BeginErrorReadLine();

                if (!processStarted)
                {
                    ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start FFmpeg process" };
                    logger.LogError("CreateFFMpegHLS Error: {ErrorMessage}", error.Message);
                    return (null, error);
                }

                logger.LogInformation("Opened ffmpeg stream for {streamName} with args \"{formattedArgs}\"", channelStatus.SMStreamInfo.Name, args);
                await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
                return (process.Id, null);
            }
            catch (Exception ex)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
                logger.LogError(ex, "CreateFFMpegHLS Error: {ErrorMessage}", error.Message);
                return (null, error);
            }
        }

        private void Process_Exited(object? sender, EventArgs e)
        {
            ChannelStatus.Shutdown = true;
            ProcessExited?.Invoke(this, new ProcessExitEventArgs { ExitCode = process?.ExitCode ?? -1 });
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                //logger.LogError("FFmpeg Error: {ErrorMessage}", e.Data);
                WriteErrorLog(e.Data);
            }
        }

        private void WriteErrorLog(string errorMessage)
        {
            try
            {
                string logDir = BuildInfo.HLSLogFolder;
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }

                string logFileName = $"{ChannelStatus.SMStreamInfo?.Name.ToUrlSafeString() ?? "NA"}_error.log";
                string logFilePath = Path.Combine(logDir, logFileName);

                using StreamWriter writer = new(logFilePath, false);
                writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {errorMessage}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to write error log");
            }
        }
    }
}
