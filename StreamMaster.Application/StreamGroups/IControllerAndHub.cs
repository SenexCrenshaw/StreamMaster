using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.StreamGroups.Queries;

namespace StreamMaster.Application.StreamGroups
{
    public interface IStreamGroupsController
    {        
    Task<ActionResult<PagedResponse<StreamGroupDto>>> GetPagedStreamGroups(QueryStringParameters Parameters);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IStreamGroupsHub
    {
        Task<PagedResponse<StreamGroupDto>> GetPagedStreamGroups(QueryStringParameters Parameters);
    }
}
