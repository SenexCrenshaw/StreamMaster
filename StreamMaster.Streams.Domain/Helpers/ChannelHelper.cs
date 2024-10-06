using System.Threading.Channels;

namespace StreamMaster.Streams.Domain.Helpers
{
    /// <summary>
    /// Provides helper methods for creating TrackedChannels.
    /// </summary>
    public static class ChannelHelper
    {
        public const int DefaultChannelCapacity = 20;

        /// <summary>
        /// Creates a tracked channel for transferring byte arrays.
        /// </summary>
        /// <param name="boundCapacity">The capacity of the bounded channel. If 0 or less, an unbounded channel will be created.</param>
        /// <returns>
        /// A <see cref="TrackedChannel"/> of byte arrays, either bounded or unbounded depending on the <paramref name="boundCapacity"/> parameter.
        /// </returns>
        /// <remarks>
        /// This method creates a tracked channel with a single reader and a single writer.
        /// - If <paramref name="boundCapacity"/> is greater than 0, a bounded tracked channel is created with the specified capacity.
        /// - If <paramref name="boundCapacity"/> is 0 or less, an unbounded tracked channel is created.
        /// </remarks>
        public static TrackedChannel GetChannel(int boundCapacity = DefaultChannelCapacity, BoundedChannelFullMode? fullMode = BoundedChannelFullMode.Wait)
        {
            return new TrackedChannel(boundCapacity, fullMode);
        }
    }
}
