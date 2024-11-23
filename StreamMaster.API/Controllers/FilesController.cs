using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.API.Interfaces;
using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Extensions;

namespace StreamMaster.API.Controllers;

public class FilesController(IOptionsMonitor<Setting> settings, ILogoService logoService) : ApiControllerBase, IFileController
{
    [AllowAnonymous]
    [Route("{filetype}/{source}")]
    public async Task<IActionResult> GetFile(string source, SMFileTypes filetype, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(source) || source == "noimage.png" || source.EndsWithIgnoreCase("default.png"))
        {
            return Redirect("/" + settings.CurrentValue.DefaultLogo);
        }

        LogoDto? logoDto = await logoService.GetLogoFromCacheAsync(source, filetype, cancellationToken).ConfigureAwait(false);
        //return logoDto == null
        //    ? source.Contains("api/files") ? Redirect("/" + settings.CurrentValue.DefaultLogo) : (IActionResult)Redirect(source)
        //    : File(logoDto.Image!, logoDto.ContentType ?? "", logoDto.FileName);

        return logoDto == null ? NotFound() : File(logoDto.Image!, logoDto.ContentType ?? "", logoDto.FileName);
    }

    [AllowAnonymous]
    [Route("smChannelLogo/{smChannelId}")]
    public async Task<IActionResult> GetSMChannelLogo(int smChannelId, CancellationToken cancellationToken)
    {

        LogoDto? logoDto = await logoService.GetLogoForChannelAsync(smChannelId, cancellationToken).ConfigureAwait(false);

        if (logoDto == null)
        {
            return Redirect("/" + settings.CurrentValue.DefaultLogo);
        }

        if (logoDto.Image.Length == 0 && !string.IsNullOrEmpty(logoDto.Url))
        {
            return Redirect(logoDto?.Url ?? logoDto!.Url);
        }

        // Next

        return File(logoDto.Image!, logoDto.ContentType ?? "", logoDto.FileName);
    }

    public bool IsLocalLogo(string Logo)
    {
        return string.IsNullOrEmpty(Logo)
          || Logo.EqualsIgnoreCase("noimage.png")
          || Logo.EqualsIgnoreCase(settings.CurrentValue.DefaultLogo)
          || Logo.EqualsIgnoreCase("/images/streammaster_logo.png");
    }
}