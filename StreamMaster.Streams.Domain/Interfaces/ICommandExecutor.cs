using StreamMaster.Domain.Configuration;

namespace StreamMaster.Streams.Domain.Interfaces;

/// <summary>
/// Represents an executor for running commands to initiate and manage streams.
/// </summary>
public interface ICommandExecutor
{
    /// <summary>
    /// Executes a command based on the specified command profile and parameters.
    /// </summary>
    /// <param name="commandProfile">The profile containing the command configuration.</param>
    /// <param name="streamUrl">The URL of the stream to process.</param>
    /// <param name="clientUserAgent">The user agent string of the client.</param>
    /// <param name="secondsIn">The starting position in seconds, if applicable.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning a <see cref="GetStreamResult"/> containing:
    /// - The stream if successfully created.
    /// - The process ID associated with the command.
    /// - An error object if the operation fails.
    /// </returns>
    GetStreamResult ExecuteCommand(
        CommandProfileDto commandProfile,
        string streamUrl,
        string clientUserAgent,
        int? secondsIn,
        CancellationToken cancellationToken = default);
}
