namespace StreamMasterApplication.Common.Interfaces;

/// <summary>
/// Provides methods to create and retrieve ICircularRingBuffer instances.
/// </summary>
public interface ICircularRingBufferFactory
{
    /// <summary>
    /// Creates a new instance of ICircularRingBuffer based on the provided child video stream and rank.
    /// </summary>
    /// <param name="childVideoStreamDto">The DTO containing details about the child video stream.</param>
    /// <param name="rank">The rank or priority of the buffer.</param>
    /// <returns>A new ICircularRingBuffer instance.</returns>
    ICircularRingBuffer CreateCircularRingBuffer(VideoStreamDto videoStreamDto, int rank);

    ///// <summary>
    ///// Retrieves an existing ICircularRingBuffer instance based on the provided stream URL.
    ///// </summary>
    ///// <param name="StreamURL">The URL of the stream for which the buffer is required.</param>
    ///// <returns>An existing ICircularRingBuffer instance, or null if not found.</returns>
    //ICircularRingBuffer? GetCircularRingBuffer(string StreamURL);
}
