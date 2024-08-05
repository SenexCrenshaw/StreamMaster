using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Models
{
    /// <summary>
    /// Defines the properties and methods for stream information in StreamMaster.
    /// </summary>
    public interface ISMStreamInfo
    {
        /// <summary>
        /// Gets or sets the client user agent.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the client user agent.
        /// </value>
        string? ClientUserAgent { get; set; }

        /// <summary>
        /// Gets or sets the command profile associated with the stream.
        /// </summary>
        /// <value>
        /// A <see cref="CommandProfileDto"/> object that represents the command profile.
        /// </value>
        /// <remarks>
        /// The <see cref="CommandProfile"/> property holds information about the command profile
        /// used for streaming. For more details, see the <see cref="CommandProfileDto"/> class.
        /// </remarks>
        CommandProfileDto CommandProfile { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the stream.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the unique identifier for the stream.
        /// </value>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the stream.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the name of the stream.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds into the stream.
        /// </summary>
        /// <value>
        /// An <see cref="int"/> representing the number of seconds into the stream.
        /// </value>
        int SecondsIn { get; set; }

        /// <summary>
        /// Gets or sets the URL of the stream.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> representing the URL of the stream.
        /// </value>
        string Url { get; set; }

        /// <summary>
        /// Gets or sets the shutdown delay for the stream.
        /// </summary>
        /// <value>
        /// An <see cref="int"/> representing the delay in seconds before the stream shuts down.
        /// </value>
        int ShutDownDelay { get; set; }

        /// <summary>
        /// Gets or sets the type of the SM channel.
        /// </summary>
        /// <value>
        /// An <see cref="SMStreamTypeEnum"/> representing the type of the SM Stream.
        /// </value>
        SMStreamTypeEnum SMStreamType { get; set; }
    }
}
