using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups.Commands;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.StreamGroups;

public interface IStreamGroupController
{
    Task<ActionResult> AddStreamGroup(AddStreamGroupRequest request);

    Task<ActionResult> DeleteStreamGroup(DeleteStreamGroupRequest request);

    Task<IActionResult> GetAllStatisticsForAllUrls();

    Task<ActionResult<StreamGroupDto>> GetStreamGroup(int id);

    Task<ActionResult<StreamGroupDto>> GetStreamGroupByStreamNumber(int StreamGroupNumber);

    Task<ContentResult> GetStreamGroupEPG(int id);

    Task<ContentResult> GetStreamGroupM3U(int id);

    Task<ActionResult<IEnumerable<StreamGroupDto>>> GetStreamGroups();

    IActionResult SimulateStreamFailure(string streamUrl);

    Task<ActionResult> UpdateStreamGroup(UpdateStreamGroupRequest request);
}

public interface IStreamGroupDB
{
    DbSet<StreamGroup> StreamGroups { get; set; }
}

public interface IStreamGroupHub
{
    public Task<StreamGroupDto?> AddStreamGroup(AddStreamGroupRequest request);

    Task<int?> DeleteStreamGroup(DeleteStreamGroupRequest request);

    Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls();

    Task<StreamGroupDto?> GetStreamGroup(int id);

    Task<StreamGroupDto?> GetStreamGroupByStreamNumber(int StreamGroupNumber);

    Task<IEnumerable<StreamGroupDto>> GetStreamGroups();

    public Task SimulateStreamFailure(string streamUrl);

    Task<StreamGroupDto?> UpdateStreamGroup(UpdateStreamGroupRequest request);
}

public interface IStreamGroupTasks
{
}

public interface IStreamGroupScoped
{ }
