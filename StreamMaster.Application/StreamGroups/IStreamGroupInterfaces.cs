using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Requests;

using StreamMaster.Application.Common.Models;
using StreamMaster.Application.StreamGroups.Commands;
using StreamMaster.Application.StreamGroups.Queries;

namespace StreamMaster.Application.StreamGroups;

public interface IStreamGroupController
{
    Task<ActionResult<string?>> GetStreamGroupVideoStreamUrl(string VideoStreamId);
    Task<ActionResult> CreateStreamGroup(CreateStreamGroupRequest request);

    Task<ActionResult> DeleteStreamGroup(DeleteStreamGroupRequest request);

    Task<ActionResult<StreamGroupDto>> GetStreamGroup(int StreamGroupNumber);

    Task<IActionResult> GetStreamGroupEPG(string encodedId);

    Task<IActionResult> GetStreamGroupM3U(string encodedId);

    Task<ActionResult<PagedResponse<StreamGroupDto>>> GetPagedStreamGroups(StreamGroupParameters parameters);


    Task<ActionResult> UpdateStreamGroup(UpdateStreamGroupRequest request);
}

public interface IStreamGroupDB
{ }

public interface IStreamGroupHub
{
    Task<string?> GetStreamGroupVideoStreamUrl(string VideoStreamId);
    Task CreateStreamGroup(CreateStreamGroupRequest request);

    Task DeleteStreamGroup(DeleteStreamGroupRequest request);

    Task FailClient(FailClientRequest request);
    Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls();

    Task<StreamGroupDto?> GetStreamGroup(int StreamGroupNumber);

    Task<PagedResponse<StreamGroupDto>> GetPagedStreamGroups(StreamGroupParameters streamGroupParameters);

    Task UpdateStreamGroup(UpdateStreamGroupRequest request);

}

public interface IStreamGroupTasks
{
}

public interface IStreamGroupScoped
{ }