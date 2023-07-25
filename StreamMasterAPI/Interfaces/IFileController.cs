using Microsoft.AspNetCore.Mvc;

using StreamMasterDomain.Enums;

namespace StreamMasterAPI.Interfaces;

public interface IFileController
{
    Task<IActionResult> GetFile(string source, SMFileTypes filetype, CancellationToken cancellationToken);
}
