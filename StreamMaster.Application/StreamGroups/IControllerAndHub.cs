using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.StreamGroups.Commands;

namespace StreamMaster.Application.StreamGroups
{
    public interface IStreamGroupsController
    {        
    Task<ActionResult<APIResponse<StreamGroupDto>>> GetPagedStreamGroups(QueryStringParameters Parameters);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IStreamGroupsHub
    {
        Task<APIResponse<StreamGroupDto>> GetPagedStreamGroups(QueryStringParameters Parameters);
    }
}
