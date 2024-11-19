namespace StreamMaster.Domain.Models;

public class SMChannelChannelLink
{
    public required int ParentSMChannelId { get; set; }
    public required SMChannel ParentSMChannel { get; set; }
    public required int SMChannelId { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public SMChannel SMChannel { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public required int Rank { get; set; }
}