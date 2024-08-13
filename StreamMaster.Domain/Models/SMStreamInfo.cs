using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Models
{
    /// <summary>
    /// Represents stream information for StreamMaster.
    /// </summary>
    [RequireAll]
    [TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
    public class SMStreamInfo : ISMStreamInfo
    {
        ///// <summary>
        ///// Initializes a new instance of the <see cref="SMStreamInfo"/> class.
        ///// </summary>
        ///// <param name="id">The unique identifier for the stream.</param>
        ///// <param name="name">The name of the stream.</param>
        ///// <param name="commandProfile">The command profile associated with the stream.</param>
        //public SMStreamInfo(string id, string name, CommandProfileDto commandProfile)
        //{
        //    Id = id;
        //    Name = name;
        //    CommandProfile = commandProfile;
        //    Url = string.Empty;
        //}

        /// <inheritdoc/>
        public required CommandProfileDto CommandProfile { get; set; }

        /// <inheritdoc/>
        public required string Id { get; set; }

        /// <inheritdoc/>
        public string? ClientUserAgent { get; set; } = null;

        /// <inheritdoc/>
        public required string Name { get; set; } = string.Empty;

        /// <inheritdoc/>
        public required string Url { get; set; } = string.Empty;

        /// <inheritdoc/>
        public int SecondsIn { get; set; } = 0;

        /// <inheritdoc/>
        //public int ShutDownDelay { get; set; } = 0;

        /// <inheritdoc/>
        public required SMStreamTypeEnum SMStreamType { get; set; }

    }
}
