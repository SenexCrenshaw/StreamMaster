using StreamMaster.Domain.Configuration;

using System.Diagnostics;
namespace StreamMaster.Streams.Factories;
public class CommandExecutor(ILogger<CommandExecutor> logger) : ICommandExecutor
{
    public (Stream? stream, int processId, ProxyStreamError? error) ExecuteCommand(CommandProfileDto commandProfile, string streamUrl, string clientUserAgent, CancellationToken cancellationToken = default)
    {
        return ExecuteCommand(commandProfile.Command, commandProfile.Parameters, commandProfile.ProfileName, streamUrl, clientUserAgent, cancellationToken);
    }

    public (Stream? stream, int processId, ProxyStreamError? error)
        ExecuteCommand(string command, string parameters, string profileName, string streamUrl, string clientUserAgent, CancellationToken cancellationToken = default)
    {
        try
        {
            string? exec = FileUtil.GetExec(command);
            if (exec == null)
            {
                logger.LogCritical("Profile {profileName} Command {command} not found", profileName, command);
                return (null, -1, new ProxyStreamError { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"{command} not found" });
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            string options = parameters.Replace("{streamUrl}", $"\"{streamUrl}\"").Replace("{clientUserAgent}", $"\"{clientUserAgent}\"");

            using Process process = new();
            ConfigureProcess(process, exec, options);
            cancellationToken.ThrowIfCancellationRequested();

            bool processStarted = process.Start();
            stopwatch.Stop();

            if (!processStarted)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start process" };
                logger.LogError("ExecuteCommandAsync Error: {ErrorMessage}", error.Message);
                return (null, -1, error);
            }

            logger.LogInformation("Opened stream with args \"{formattedArgs}\" in {ElapsedMilliseconds} ms", options, stopwatch.ElapsedMilliseconds);
            return (process.StandardOutput.BaseStream, process.Id, null);
        }
        catch (OperationCanceledException ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.OperationCancelled, Message = "Operation was cancelled" };
            logger.LogError(ex, "ExecuteCommandAsync Error: {ErrorMessage}", error.Message);
            return (null, -1, error);
        }
        catch (Exception ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
            logger.LogError(ex, "ExecuteCommandAsync Error: {ErrorMessage}", error.Message);
            return (null, -1, error);
        }
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