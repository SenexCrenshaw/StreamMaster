using AutoMapper.Configuration.Annotations;

using MessagePack;

using StreamMaster.Domain.Attributes;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

using KeyAttribute = System.ComponentModel.DataAnnotations.KeyAttribute;

namespace StreamMaster.Domain.Models
{
    /// <summary>
    /// Represents a channel in the system.
    /// </summary>
    public class SMChannel : ISMChannel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the SMChannel.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets the API name for this class.
        /// </summary>
        public static string APIName => "SMChannels";

        /// <summary>
        /// Gets or sets the name of the command profile.
        /// </summary>
        [Column(TypeName = "citext")]
        public string CommandProfileName { get; set; } = "Default";

        /// <summary>
        /// Gets or sets a value indicating whether the channel is hidden.
        /// </summary>
        public bool IsHidden { get; set; } = false;

        /// <summary>
        /// Gets or sets the base stream ID for the channel.
        /// </summary>
        public string BaseStreamID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the M3U file associated with this channel.
        /// </summary>
        public int M3UFileId { get; set; }

        /// <summary>
        /// Gets or sets the channel number.
        /// </summary>
        public int ChannelNumber { get; set; } = 0;

        /// <summary>
        /// Gets or sets the time shift value for the channel.
        /// </summary>
        public int TimeShift { get; set; } = 0;

        /// <summary>
        /// Gets or sets the group associated with the channel.
        /// </summary>
        [Column(TypeName = "citext")]
        public string Group { get; set; } = "Dummy";

        /// <summary>
        /// Gets or sets the EPG ID for the channel.
        /// </summary>
        [Column(TypeName = "citext")]
        public string EPGId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the logo URL for the channel.
        /// </summary>
        [Column(TypeName = "citext")]
        public string Logo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the channel.
        /// </summary>
        [Column(TypeName = "citext")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the client user agent string for the channel.
        /// </summary>
        [Column(TypeName = "citext")]
        public string? ClientUserAgent { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the station ID associated with the channel.
        /// </summary>
        [Column(TypeName = "citext")]
        public string StationId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the group title for the channel.
        /// </summary>
        [Column(TypeName = "citext")]
        public string GroupTitle { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the channel is a system channel.
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Gets or sets the type of the SMChannel.
        /// </summary>
        public SMChannelTypeEnum SMChannelType { get; set; } = SMChannelTypeEnum.Regular;

        // Ignore properties in serialization and mapping
        [Ignore, JsonIgnore, IgnoreMember, IgnoreMap, XmlIgnore, TsIgnore]
        public ICollection<SMChannelStreamLink> SMStreams { get; set; } = [];

        [Ignore, JsonIgnore, IgnoreMember, IgnoreMap, XmlIgnore, TsIgnore]
        public ICollection<SMChannelChannelLink> SMChannels { get; set; } = [];

        [Ignore, JsonIgnore, IgnoreMember, IgnoreMap, XmlIgnore, TsIgnore]
        public ICollection<StreamGroupSMChannelLink> StreamGroups { get; set; } = [];
    }
}
