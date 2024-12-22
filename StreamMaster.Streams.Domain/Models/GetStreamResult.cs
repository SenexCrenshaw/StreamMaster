namespace StreamMaster.Streams.Domain.Models;
/// <summary>
/// Represents the result of a stream creation operation.
/// </summary>
/// <param name="Stream"> Gets or sets the created stream, if successful. </param>
/// <param name="ProcessId"> Gets or sets the process ID associated with the stream. </param>
/// <param name="Error"> Gets or sets the error information, if applicable. </param>
public record GetStreamResult(Stream? Stream, int ProcessId, ProxyStreamError? Error);
