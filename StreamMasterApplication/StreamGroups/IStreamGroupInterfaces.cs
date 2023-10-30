using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.StreamGroups.Queries;

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

    Task<ActionResult<PagedResponse<StreamGroupDto>>> GetPagedStreamGroups(StreamGroupParameters parameters);


    Task<ActionResult> UpdateStreamGroup(UpdateStreamGroupRequest request);
}

public interface IStreamGroupDB
{}

public interface IStreamGroupHub
{
    Task CreateStreamGroup(CreateStreamGroupRequest request);

    Task DeleteStreamGroup(DeleteStreamGroupRequest request);

    Task FailClient(FailClientRequest request);
    Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls();

    Task<StreamGroupDto?> GetStreamGroup(int StreamGroupNumber);

    Task<EPGGuide> GetStreamGroupEPGForGuide(int StreamGroupNumber);

    Task<PagedResponse<StreamGroupDto>> GetPagedStreamGroups(StreamGroupParameters streamGroupParameters);

    Task UpdateStreamGroup(UpdateStreamGroupRequest request);

}

public interface IStreamGroupTasks
{
}

public interface IStreamGroupScoped
{ }