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
