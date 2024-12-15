using System.Diagnostics;
using System.Text;

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

            //if (secondsIn.HasValue && secondsIn.Value != 0)
            //{
            //    streamUrl = streamUrl.Replace("{streamUrl}", $"-ss {secondsIn} {{streamUrl}}");
            //    streamUrl = $"-ss {secondsIn} {streamUrl}";
            //}

            string options = BuildCommand(commandProfile.Parameters, clientUserAgent, streamUrl, secondsIn);
            //string options = cmd;// streamUrl.Contains("://")
            //? cmd
            //: $"-hide_banner -loglevel error  -i \"{streamUrl}\" {commandProfile.Parameters} -f mpegts pipe:1";

            using Process process = new();
            ConfigureProcess(process, exec, options);
            cancellationToken.ThrowIfCancellationRequested();

            //process.ErrorDataReceived += (sender, e) =>
            //{
            //    if (!string.IsNullOrWhiteSpace(e.Data))
            //    {
            //        logger.LogError("Process stderr: {Error}", e.Data);
            //    }
            //};

            bool processStarted = process.Start();

            if (!processStarted)
            {
                ProxyStreamError error = new() { ErrorCode = ProxyStreamErrorCode.ProcessStartFailed, Message = "Failed to start process" };
                logger.LogError("Error: {ErrorMessage}", error.Message);
                return (null, -1, error);
            }

            // Save stderr to a file named after the process's PID
            string stderrFilePath = Path.Combine(BuildInfo.CommandErrorFolder, $"stderr_{process.Id}.log");
            using StreamWriter errorWriter = new(stderrFilePath, append: true, Encoding.UTF8);
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    errorWriter.WriteLine(e.Data);
                    errorWriter.Flush(); // Ensure immediate write
                    //logger.LogError("Process stderr: {Error}", e.Data);
                }
            };

            process.BeginErrorReadLine(); // Start reading stderr asynchronously

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

    private static string BuildCommand(string command, string clientUserAgent, string streamUrl, int? secondsIn)
    {
        // Create the secondsIn string if it's provided
        string s = secondsIn.HasValue ? $"-ss {secondsIn} " : "";

        // Replace placeholders for clientUserAgent and streamUrl
        command = command.Replace("{clientUserAgent}", '"' + clientUserAgent + '"')
                         .Replace("{streamUrl}", '"' + streamUrl + '"');

        // If secondsIn is provided, insert it right before the "-i" option
        if (secondsIn.HasValue)
        {
            // Insert the secondsIn string just before the first occurrence of "-i"
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
}