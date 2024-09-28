using System.Diagnostics;
namespace StreamMaster.Streams.Factories;
public class CommandExecutor(ILogger<CommandExecutor> logger) : ICommandExecutor
{
    public (Stream? stream, int processId, ProxyStreamError? error)
        ExecuteCommand(CommandProfileDto commandProfile, string streamUrl, string clientUserAgent, int? secondsIn, CancellationToken cancellationToken = default)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
            string? exec = FileUtil.GetExec(commandProfile.Command);
            if (exec == null)
            {
                logger.LogCritical("Command \"{command}\" not found", commandProfile.Command);
                return (null, -1, new ProxyStreamError { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"{commandProfile.Command} not found" });
            }

            //string prefix = "";// "-hide_banner -loglevel error";
            if (secondsIn.HasValue && secondsIn.Value != 0)
            {
                streamUrl = $"-ss {secondsIn} {streamUrl}";
            }

            string cmd = BuildCommand(commandProfile.Parameters, clientUserAgent, streamUrl);

            string options = streamUrl.Contains("://")
            ? cmd
            : $"-hide_banner -loglevel error  -i \"{streamUrl}\" {commandProfile.Parameters} -f mpegts pipe:1";

            using Process process = new();
            ConfigureProcess(process, exec, options);
            cancellationToken.ThrowIfCancellationRequested();

            bool processStarted = process.Start();

            if (!processStarted)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start process" };
                logger.LogError("Error: {ErrorMessage}", error.Message);
                return (null, -1, error);
            }
            stopwatch.Stop();

            logger.LogInformation("Opened command with args \"{options}\" in {ElapsedMilliseconds} ms", commandProfile.Command + ' ' + commandProfile.Parameters, stopwatch.ElapsedMilliseconds);
            return (process.StandardOutput.BaseStream, process.Id, null);
        }
        catch (OperationCanceledException ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.OperationCancelled, Message = "Operation was cancelled" };
            logger.LogError(ex, "Error: {ErrorMessage}", error.Message);
            return (null, -1, error);
        }
        catch (Exception ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
            logger.LogError(ex, "Error: {ErrorMessage}", error.Message);
            return (null, -1, error);
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    private static string BuildCommand(string command, string clientUserAgent, string streamUrl)
    {
        return command.Replace("{clientUserAgent}", '"' + clientUserAgent + '"')
                      .Replace("{streamUrl}", '"' + streamUrl + '"');
    }

    private static void ConfigureProcess(Process process, string commandExec, string formattedArgs)
    {
        process.StartInfo.FileName = commandExec;
        process.StartInfo.Arguments = formattedArgs;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
    }
}