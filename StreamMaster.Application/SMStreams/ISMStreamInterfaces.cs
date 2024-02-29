using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.SMStreams;

public interface ISMStreamController
{
    Task<ActionResult<PagedResponse<SMStreamDto>>> GetPagedSMStreams([FromQuery] SMStreamParameters Parameters);
}

public interface ISMStreamHub
{

}