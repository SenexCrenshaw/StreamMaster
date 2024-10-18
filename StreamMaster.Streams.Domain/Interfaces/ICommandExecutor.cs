using StreamMaster.Domain.Configuration;
namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface ICommandExecutor
    {
        (Stream? stream, int processId, ProxyStreamError? error) ExecuteCommand(CommandProfileDto commandProfile, string streamUrl, string clientUserAgent, int? secondsIn, CancellationToken cancellationToken = default);
    }
}