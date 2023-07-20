using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.StreamGroups.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups;

public interface IStreamGroupController
{
    Task<ActionResult> AddStreamGroup(AddStreamGroupRequest request);

    Task<ActionResult> DeleteStreamGroup(DeleteStreamGroupRequest request);

    Task<ActionResult> FailClient(FailClientRequest request);

    Task<IActionResult> GetAllStatisticsForAllUrls();

    Task<ActionResult<StreamGroupDto>> GetStreamGroup(int StreamGroupNumber);

    Task<ActionResult<StreamGroupDto>> GetStreamGroupByStreamNumber(int StreamGroupNumber);

    Task<IActionResult> GetStreamGroupEPG(string encodedId);

    Task<ActionResult<EPGGuide>> GetStreamGroupEPGForGuide(int StreamGroupNumber);

    Task<IActionResult> GetStreamGroupM3U(string encodedId);

    Task<ActionResult<IEnumerable<StreamGroupDto>>> GetStreamGroups();

    Task<IActionResult> GetStreamGroupVideoStream(string encodedId, string name, CancellationToken cancellationToken);

    IActionResult SimulateStreamFailure(string streamUrl);

    Task<ActionResult> UpdateStreamGroup(UpdateStreamGroupRequest request);
}

public interface IStreamGroupDB
{
    DbSet<StreamGroup> StreamGroups { get; set; }

    Task<bool> AddChannelGroupToStreamGroupAsync(int streamGroupId, int channelGroupId, CancellationToken cancellationToken);

    Task AddOrUpdatVideoStreamToStreamGroupAsync(int streamgroupId, int childId, bool isReadOnly, CancellationToken cancellationToken);

    Task<bool> DeleteStreamGroupsync(int streamGroupId, CancellationToken cancellationToken);

    Task<List<StreamGroup>> GetAllStreamGroupsWithRelatedEntitiesAsync(CancellationToken cancellationToken);

    Task<StreamGroupDto?> GetStreamGroupDto(int streamGroupId, string Url, CancellationToken cancellationToken = default);

    Task<List<StreamGroupDto>> GetStreamGroupDtos(string Url, CancellationToken cancellationToken = default);

    Task<StreamGroup> GetStreamGroupWithRelatedEntitiesByIdAsync(int streamGroupId, CancellationToken cancellationToken);

    Task<StreamGroupDto> GetStreamGroupWithRelatedEntitiesByStreamGroupNumberAsync(int streamGroupNumber, CancellationToken cancellationToken);

    Task<bool> RemoveChildVideoStreamFromStreamGroupAsync(int streamGroupId, int videoStreamId, CancellationToken cancellationToken);

    Task<int> RemoveChildVideoStreamsFromStreamGroupAsync(int streamGroupId, List<int> videoStreamIds, CancellationToken cancellationToken);

    Task<(int added, int removed)> SynchronizeChildVideoStreamsInStreamGroupAsync(int streamGroupId, List<VideoStreamIsReadOnly> validVideoStreams, CancellationToken cancellationToken);

    Task<StreamGroupDto?> UpdateStreamGroupAsync(UpdateStreamGroupRequest request, string Url, CancellationToken cancellationToken);
}

public interface IStreamGroupHub
{
    Task<StreamGroupDto?> AddStreamGroup(AddStreamGroupRequest request);

    Task<int?> DeleteStreamGroup(DeleteStreamGroupRequest request);

    Task FailClient(FailClientRequest request);

    Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls();

    Task<StreamGroupDto?> GetStreamGroup(int StreamGroupNumber);

    Task<StreamGroupDto?> GetStreamGroupByStreamNumber(int StreamGroupNumber);

    Task<EPGGuide> GetStreamGroupEPGForGuide(int StreamGroupNumber);

    Task<IEnumerable<StreamGroupDto>> GetStreamGroups();

    public Task SimulateStreamFailure(string streamUrl);

    Task<StreamGroupDto?> UpdateStreamGroup(UpdateStreamGroupRequest request);
}

public interface IStreamGroupTasks
{
}

public interface IStreamGroupScoped
{ }
