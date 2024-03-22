namespace StreamMaster.Application.ChannelGroups;

public interface IChannelGroupTasks
{
    ValueTask UpdateChannelGroupCounts(CancellationToken cancellationToken = default);
}