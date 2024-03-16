using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.SMStreams.Commands;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.SMStreams;

public interface ISMStreamController
{
    Task<ActionResult<PagedResponse<SMStreamDto>>> GetPagedSMStreams([FromQuery] SMStreamParameters Parameters);
    Task<ActionResult<bool>> ToggleSMStreamVisible(ToggleSMStreamVisibleRequest request);
}

public interface ISMStreamHub
{

}