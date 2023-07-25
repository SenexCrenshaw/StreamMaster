namespace StreamMasterApplication.Common.Models
{
    /// <summary>
    /// Defines the configuration options for a video streamer.
    /// </summary>
    public class ClientStreamerConfiguration
    {
        public ClientStreamerConfiguration(string videoStreamId, string clientUserAgent, CancellationToken cancellationToken)
        {
            ClientId = Guid.NewGuid();
            VideoStreamId = videoStreamId;
            ClientUserAgent = clientUserAgent;
            CancellationToken = cancellationToken;
            CancellationTokenSource = new CancellationTokenSource();
        }

        public Func<ICircularRingBuffer> BufferDelegate { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public Guid ClientId { get; set; }

        public string ClientUserAgent { get; set; }

        public IRingBufferReadStream? ReadBuffer { get; set; }

        public string VideoStreamId { get; set; }

        public string VideoStreamName { get; set; }
    }
}
