using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.StreamGroups.CommandsOld;
using StreamMaster.Domain.Requests;

namespace StreamMaster.Application.StreamGroups;

public interface IStreamGroupController
{
    Task<ActionResult<string?>> GetStreamGroupVideoStreamUrl(string VideoStreamId);
    Task<ActionResult> CreateStreamGroup(CreateStreamGroupRequest request);

    Task<ActionResult> DeleteStreamGroup(DeleteStreamGroupRequest request);

    Task<ActionResult<StreamGroupDto>> GetStreamGroup(int StreamGroupNumber);

    Task<IActionResult> GetStreamGroupEPG(string encodedId);

    Task<IActionResult> GetStreamGroupM3U(string encodedId);



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


    Task<StreamGroupDto?> GetStreamGroup(int StreamGroupNumber);


    Task UpdateStreamGroup(UpdateStreamGroupRequest request);

}

public interface IStreamGroupTasks
{
}

public interface IStreamGroupScoped
{ }