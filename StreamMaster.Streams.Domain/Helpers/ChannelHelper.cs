using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Helpers
{
    /// <summary>
    /// Provides helper methods for creating channels.
    /// </summary>
    public static class ChannelHelper
    {
        public const int DefaultChannelCapacity = 100;
        /// <summary>
        /// Creates a channel for transferring byte arrays.
        /// </summary>
        /// <param name="boundCapacity">The capacity of the bounded channel. If 0 or less, an unbounded channel will be created.</param>
        /// <returns>
        /// A <see cref="Channel{T}"/> of byte arrays, either bounded or unbounded depending on the <paramref name="boundCapacity"/> parameter.
        /// </returns>
        /// <remarks>
        /// This method creates a channel with a single reader and a single writer.
        /// - If <paramref name="boundCapacity"/> is greater than 0, a bounded channel is created with the specified capacity.
        /// - If <paramref name="boundCapacity"/> is 0 or less, an unbounded channel is created.
        /// </remarks>
        public static Channel<byte[]> GetChannel(int boundCapacity = DefaultChannelCapacity)
        {
            return boundCapacity > 0
                ? Channel.CreateBounded<byte[]>(new BoundedChannelOptions(boundCapacity) { SingleReader = true, SingleWriter = true, FullMode = BoundedChannelFullMode.Wait })
                : Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });
        }
    }
}
