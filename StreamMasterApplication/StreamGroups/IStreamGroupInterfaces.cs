using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.StreamGroups.Queries;
using StreamMasterDomain.Models;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.StreamGroups;

public interface IStreamGroupController
{
    Task<ActionResult> CreateStreamGroup(CreateStreamGroupRequest request);

    Task<ActionResult> DeleteStreamGroup(DeleteStreamGroupRequest request);

    Task<ActionResult<StreamGroupDto>> GetStreamGroup(int StreamGroupNumber);

    Task<IActionResult> GetStreamGroupEPG(string encodedId);

    Task<ActionResult<EPGGuide>> GetStreamGroupEPGForGuide(int StreamGroupNumber);

    Task<IActionResult> GetStreamGroupM3U(string encodedId);

    Task<ActionResult<PagedResponse<StreamGroupDto>>> GetStreamGroups(StreamGroupParameters parameters);


    Task<ActionResult> UpdateStreamGroup(UpdateStreamGroupRequest request);
}

public interface IStreamGroupDB
{
    //DbSet<StreamGroup> StreamGroups { get; set; }

    //Task<bool> AddChannelGroupToStreamGroupAsync(int streamGroupId, int channelGroupId, CancellationToken cancellationToken);

    //Task AddOrUpdatVideoStreamToStreamGroupAsync(int streamgroupId, string childId, bool isReadOnly, CancellationToken cancellationToken);

    //Task<bool> DeleteStreamGroupsync(int streamGroupId, CancellationToken cancellationToken);

    ////Task<List<StreamGroup>> GetAllStreamGroupsWithRelatedEntitiesAsync(CancellationToken cancellationToken);

    //Task<StreamGroupDto?> GetStreamGroupDto(int streamGroupId, string Url, CancellationToken cancellationToken = default);

    //Task<StreamGroupDto?> GetStreamGroupDtoByStreamGroupNumber(int streamGroupNumber, string Url, CancellationToken cancellationToken = default);

    ////Task<List<StreamGroup>> GetAllStreamGroupsWithRelatedEntitiesAsync(CancellationToken cancellationToken);
    //Task<List<StreamGroupDto>> GetStreamGroupDtos(string Url, CancellationToken cancellationToken = default);

    //Task<List<StreamGroupDto>> GetStreamGroupsByVideoStreamIdsAsync(List<string> videoStreamIds, string url, CancellationToken cancellationToken);

    //Task<List<VideoStream>> GetVideoStreamsByNamePatternAsync(string pattern, CancellationToken cancellationToken);

    ////Task<StreamGroup> GetStreamGroupWithRelatedEntitiesByIdAsync(int streamGroupId, CancellationToken cancellationToken);

    ////Task<StreamGroupDto> GetStreamGroupWithRelatedEntitiesByStreamGroupNumberAsync(int streamGroupNumber, CancellationToken cancellationToken);

    //Task<bool> RemoveChildVideoStreamFromStreamGroupAsync(int streamGroupId, string videoStreamId, CancellationToken cancellationToken);

    //Task<int> RemoveChildVideoStreamsFromStreamGroupAsync(int streamGroupId, List<string> videoStreamIds, CancellationToken cancellationToken);

    //Task<(int added, int removed)> SynchronizeChildVideoStreamsInStreamGroupAsync(int streamGroupId, List<VideoStreamIsReadOnly> validVideoStreams, CancellationToken cancellationToken);

    //Task<StreamGroupDto?> UpdateStreamGroupAsync(UpdateStreamGroupRequest request, string Url, CancellationToken cancellationToken);
}

public interface IStreamGroupHub
{
    Task CreateStreamGroup(CreateStreamGroupRequest request);

    Task DeleteStreamGroup(DeleteStreamGroupRequest request);

    Task FailClient(FailClientRequest request);
    Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls();

    Task<StreamGroupDto?> GetStreamGroup(int StreamGroupNumber);

    Task<EPGGuide> GetStreamGroupEPGForGuide(int StreamGroupNumber);

    Task<PagedResponse<StreamGroupDto>> GetStreamGroups(StreamGroupParameters streamGroupParameters);



    Task UpdateStreamGroup(UpdateStreamGroupRequest request);


}

public interface IStreamGroupTasks
{
}

public interface IStreamGroupScoped
{ }