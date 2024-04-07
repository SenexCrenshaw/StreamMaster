using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMChannelStreamLinks.Queries;

namespace StreamMaster.Application.SMChannelStreamLinks
{
    public interface ISMChannelStreamLinksController
    {        
        Task<ActionResult<List<SMStreamDto>>> GetSMChannelStreams(int SMChannelId);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISMChannelStreamLinksHub
    {
        Task<List<SMStreamDto>> GetSMChannelStreams(int SMChannelId);
    }
}
