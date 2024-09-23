using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Enums;

namespace StreamMaster.API.Interfaces;

public interface IFileController
{
    Task<IActionResult> GetFile(string source, SMFileTypes filetype, CancellationToken cancellationToken);
}
