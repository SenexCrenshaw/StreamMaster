using System.Diagnostics;
using System.Text;

namespace StreamMaster.Streams.Factories;

/// <summary>
/// Executes commands based on the provided profiles and manages process lifecycles.
/// </summary>
public class CommandExecutor(ILogger<CommandExecutor> logger) : ICommandExecutor, IDisposable
{
    private StreamWriter? errorWriter;
    private Process? _process;
    private bool _disposed;

    /// <inheritdoc/>
    public GetStreamResult ExecuteCommand(CommandProfileDto commandProfile, string streamUrl, string clientUserAgent, int? secondsIn, CancellationToken cancellationToken = default)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            string? exec = FileUtil.GetExec(commandProfile.Command);
            if (exec == null)
            {
                logger.LogCritical("Command \"{command}\" not found", commandProfile.Command);
                return new GetStreamResult(null, -1, new ProxyStreamError { ErrorCode = ProxyStreamErrorCode.FileNotFound, Message = $"{commandProfile.Command} not found" });
            }

            string options = BuildCommand(commandProfile.Parameters, clientUserAgent, streamUrl, secondsIn);

            _process = new Process();
            ConfigureProcess(_process, exec, options);

            cancellationToken.ThrowIfCancellationRequested();

            if (!_process.Start())
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start process" };
                logger.LogError("Error: {ErrorMessage}", error.Message);
                return new GetStreamResult(null, -1, error);
            }

            string stderrFilePath = Path.Combine(BuildInfo.CommandErrorFolder, $"stderr_{_process.Id}.log");
            errorWriter = new StreamWriter(stderrFilePath, append: true, Encoding.UTF8);

            _process.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    lock (errorWriter) // Ensure thread-safe writes
                    {
                        errorWriter.WriteLine(e.Data);
                        errorWriter.Flush();
                    }
                }
            };
            _process.BeginErrorReadLine();
            _process.Exited += _process_Exited;

            stopwatch.Stop();
            logger.LogInformation("Opened command with args \"{options}\" in {ElapsedMilliseconds} ms", commandProfile.Command + ' ' + commandProfile.Parameters, stopwatch.ElapsedMilliseconds);

            return new GetStreamResult(_process.StandardOutput.BaseStream, _process.Id, null);
        }
        catch (OperationCanceledException ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.OperationCancelled, Message = "Operation was cancelled" };
            logger.LogError(ex, "Error: {ErrorMessage}", error.Message);
            return new GetStreamResult(null, -1, error);
        }
        catch (Exception ex)
        {
            ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.UnknownError, Message = ex.Message };
            logger.LogError(ex, "Error: {ErrorMessage}", error.Message);
            return new GetStreamResult(null, -1, error);
        }
        finally
        {
            stopwatch.Stop();

        }
    }

    private void _process_Exited(object? sender, EventArgs e)
    {
        if (_process != null)
        {
            try
            {
                _process.WaitForExit(); // Ensure process completes before disposing resources
                _process.CancelErrorRead();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error waiting for process to exit.");
            }
        }

        if (errorWriter != null)
        {
            try
            {
                errorWriter.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error disposing error writer.");
            }
        }

        try
        {
            _process?.Dispose();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error disposing process.");
        }
    }

    private static string BuildCommand(string command, string clientUserAgent, string streamUrl, int? secondsIn)
    {
        string s = secondsIn.HasValue ? $"-ss {secondsIn} " : "";

        command = command.Replace("{clientUserAgent}", '"' + clientUserAgent + '"')
                         .Replace("{streamUrl}", '"' + streamUrl + '"');

        if (secondsIn.HasValue)
        {
            int index = command.IndexOf("-i ");
            if (index >= 0)
            {
                command = command.Insert(index, s);
            }
        }

        return command;
    }

    private static void ConfigureProcess(Process process, string commandExec, string formattedArgs)
    {
        process.StartInfo.FileName = commandExec;
        process.StartInfo.Arguments = formattedArgs;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
    }

    /// <summary>
    /// Disposes the process and cleans up resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (_process != null)
        {
            try
            {
                if (!_process.HasExited)
                {
                    _process.Kill();
                }
                _process.Dispose();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error disposing process.");
            }
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
