using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirect.Domain.Enums;

namespace StreamMasterAPI.Interfaces;

public interface IFileController
{
    Task<IActionResult> GetFile(string source, SMFileTypes filetype, CancellationToken cancellationToken);
}
