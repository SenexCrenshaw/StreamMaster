using StreamMaster.Application.Interfaces;
using StreamMaster.Domain.Configuration;
using StreamMaster.Streams.Domain.Args;

namespace StreamMaster.Streams.Streams;

public class HLSHandler(ILoggerFactory loggerFactory, ICryptoService cryptoService, IM3U8ChannelStatus channelStatus, IOptionsMonitor<Setting> intSettings, IOptionsMonitor<HLSSettings> intHLSSettings)
     : IHLSHandler
{
    internal readonly ILogger<HLSHandler> logger = loggerFactory.CreateLogger<HLSHandler>();
    internal readonly ILogger<HLSRunner> HLSRunnerlogger = loggerFactory.CreateLogger<HLSRunner>();
    internal readonly CancellationTokenSource HLSCancellationTokenSource = new();
    internal bool Started;

    private HLSRunner? _hlsRunner = null;
    public HLSRunner HLSRunner
    {
        get
        {
            _hlsRunner ??= new HLSRunner(HLSRunnerlogger, channelStatus, cryptoService, intSettings, intHLSSettings);
            return _hlsRunner;
        }
    }

    public IM3U8ChannelStatus ChannelStatus => channelStatus;
    public void Dispose()
    {
        ProcessHelper.KillProcessById(HLSRunner.ProcessId);
        HLSCancellationTokenSource.Cancel();
        GC.SuppressFinalize(this);
    }

    public event EventHandler<ProcessExitEventArgs> ProcessExited;
    public Stream? Stream { get; }
    public void Start()
    {
        if (Started)
        {
            return;
        }

        logger.LogInformation("Starting HLSHandler for {Name}", ChannelStatus.SMChannel.Name);

        Task backgroundTask = HLSRunner.HLSStartStreamingInBackgroundAsync(HLSCancellationTokenSource.Token);

        HLSRunner.ProcessExited += (sender, args) =>
        {
            logger.LogInformation("Process Exited for {Name} with exit code {ExitCode}", ChannelStatus.SMChannel.Name, args.ExitCode);
            //Stop();
            ProcessExited?.Invoke(this, args);
        };
        Started = true;
    }
    public void Stop()
    {
        Started = false;
        logger.LogInformation("Stopping HLSHandler for {Name}", ChannelStatus.SMChannel.Name);
        HLSCancellationTokenSource.Cancel();
        ProcessHelper.KillProcessById(HLSRunner.ProcessId);

        string directory = Path.Combine(BuildInfo.HLSOutputFolder, ChannelStatus.M3U8Directory);
        DirectoryHelper.DeleteDirectory(directory);
    }
}