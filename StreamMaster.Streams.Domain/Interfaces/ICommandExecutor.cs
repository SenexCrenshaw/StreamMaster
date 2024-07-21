using StreamMaster.Domain.Configuration;

public interface ICommandExecutor
{
    (Stream? stream, int processId, ProxyStreamError? error) ExecuteCommand(CommandProfileDto commandProfile, string streamUrl, string clientUserAgent, CancellationToken cancellationToken = default);
    (Stream? stream, int processId, ProxyStreamError? error) ExecuteCommand(string command, string parameters, string smStreamId, string streamUrl, string clientUserAgent, CancellationToken cancellationToken = default);
}