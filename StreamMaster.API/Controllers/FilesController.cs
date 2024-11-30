using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Extensions;

namespace StreamMaster.API.Controllers;

[V1ApiController("api/[controller]")]
public class FilesController(IOptionsMonitor<Setting> settings, ILogoService logoService) : ControllerBase
{
    [AllowAnonymous]
    [Route("{source}")]
    public async Task<IActionResult> GetLogo(string source, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(source) || source == "noimage.png" || source.EndsWithIgnoreCase("default.png"))
        {
            return Redirect("/" + settings.CurrentValue.DefaultLogo);
        }

        (FileStream? fileStream, string? FileName, string? ContentType) = await logoService.GetLogoAsync(source, cancellationToken).ConfigureAwait(false);

        return fileStream == null ? Redirect("/" + FileName ?? settings.CurrentValue.DefaultLogo) : File(fileStream, ContentType ?? "application/octet-stream", FileName);
    }

    [AllowAnonymous]
    [Route("sm/{smChannelId}")]
    public async Task<IActionResult> GetSMChannelLogo(int smChannelId, CancellationToken cancellationToken)
    {
        if (smChannelId < 0)
        {
            return Redirect("/" + settings.CurrentValue.DefaultLogo);
        }

        (FileStream? fileStream, string? FileName, string? ContentType) = await logoService.GetLogoForChannelAsync(smChannelId, cancellationToken).ConfigureAwait(false);

        return fileStream == null ? Redirect(FileName ?? "/" + settings.CurrentValue.DefaultLogo) : File(fileStream, ContentType ?? "application/octet-stream", FileName);
    }
    [AllowAnonymous]
    [Route("pr/{source}")]
    public async Task<IActionResult> GetProgramLogo(string source, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(source) || source == "noimage.png" || source.EndsWithIgnoreCase("default.png"))
        {
            return Redirect("/" + settings.CurrentValue.DefaultLogo);
        }

        (FileStream? fileStream, string? FileName, string? ContentType) = await logoService.GetProgramLogoAsync(source, cancellationToken).ConfigureAwait(false);

        return fileStream == null ? Redirect(FileName ?? "/" + settings.CurrentValue.DefaultLogo) : File(fileStream, ContentType ?? "application/octet-stream", FileName);
    }

    [AllowAnonymous]
    [Route("cu/{source}")]
    public async Task<IActionResult> GetCustomLogo(string source, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(source) || source == "noimage.png" || source.EndsWithIgnoreCase("default.png"))
        {
            return Redirect("/" + settings.CurrentValue.DefaultLogo);
        }

        (FileStream? fileStream, string? FileName, string? ContentType) = await logoService.GetCustomLogoAsync(source, cancellationToken).ConfigureAwait(false);

        return fileStream == null ? Redirect(FileName ?? "/" + settings.CurrentValue.DefaultLogo) : File(fileStream, ContentType ?? "application/octet-stream", FileName);
    }
}