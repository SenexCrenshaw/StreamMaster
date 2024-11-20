
namespace StreamMaster.Domain.Models
{
    public interface ISMChannel
    {
        string BaseStreamID { get; set; }
        int ChannelNumber { get; set; }
        string CommandProfileName { get; set; }
        string EPGId { get; set; }
        string Group { get; set; }
        string GroupTitle { get; set; }
        int Id { get; set; }
        bool IsHidden { get; set; }
        bool IsSystem { get; set; }
        string Logo { get; set; }
        int M3UFileId { get; set; }
        string Name { get; set; }
        ICollection<SMChannelChannelLink> SMChannels { get; set; }
        SMChannelTypeEnum SMChannelType { get; set; }
        ICollection<SMChannelStreamLink> SMStreams { get; set; }
        string StationId { get; set; }
        ICollection<StreamGroupSMChannelLink> StreamGroups { get; set; }
        int TimeShift { get; set; }
    }
}