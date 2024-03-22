using Microsoft.AspNetCore.Mvc;

namespace StreamMaster.Application.SMStreams;

public interface ISMStreamsController
{
    Task<ActionResult<DefaultAPIResponse?>> ToggleSMStreamVisibleById(string id);
    Task<ActionResult<APIResponse<SMStreamDto>?>> GetPagedSMStreams(SMStreamParameters parameters);
}

public interface ISMStreamsHub
{
    Task<DefaultAPIResponse?> ToggleSMStreamVisibleById(string id);
    Task<APIResponse<SMStreamDto?>> GetPagedSMStreams(SMStreamParameters parameters);
}