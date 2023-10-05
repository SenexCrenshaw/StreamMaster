namespace StreamMasterApplication.Common.Models
{
    /// <summary>
    /// Defines the configuration options for a video streamer.
    /// </summary>
    public class ClientStreamerConfiguration(string videoStreamId, string clientUserAgent, string clientIPAddress, CancellationToken cancellationToken)
    {
        public Func<ICircularRingBuffer> BufferDelegate { get; set; }
        public CancellationToken CancellationToken { get; set; } = cancellationToken;
        public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
        public string ClientIPAddress { get; set; } = clientIPAddress;
        public Guid ClientId { get; set; } = Guid.NewGuid();

        public string ClientUserAgent { get; set; } = clientUserAgent;

        public IRingBufferReadStream? ReadBuffer { get; set; }

        public string VideoStreamId { get; set; } = videoStreamId;

        public string VideoStreamName { get; set; }
    }
}
