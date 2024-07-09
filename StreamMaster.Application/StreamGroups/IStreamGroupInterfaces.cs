using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.StreamGroups.Commands;

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
