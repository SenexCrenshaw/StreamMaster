namespace StreamMasterApplication.Common.Models
{
    /// <summary>
    /// Defines the configuration options for a video streamer.
    /// </summary>
    public class ClientStreamerConfiguration
    {
        public ClientStreamerConfiguration(int videoStreamId, string clientUserAgent, CancellationToken cancellationToken)
        {
            ClientId = Guid.NewGuid();
            VideoStreamId = videoStreamId;
            ClientUserAgent = clientUserAgent;
            CancellationToken = cancellationToken;
        }

        public Func<ICircularRingBuffer> BufferDelegate { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public Guid ClientId { get; set; }

        public string ClientUserAgent { get; set; }

        public IRingBufferReadStream? ReadBuffer { get; set; }

        public int VideoStreamId { get; set; }
    }
}
