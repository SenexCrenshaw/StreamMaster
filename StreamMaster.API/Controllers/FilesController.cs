﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Common.Extensions;
using StreamMaster.Application.StreamGroups;
using StreamMaster.Domain.Extensions;

namespace StreamMaster.API.Controllers;

[V1ApiController("api/[controller]")]
public class FilesController(ILogger<FilesController> logger, IHttpContextAccessor httpContextAccessor, IOptionsMonitor<Setting> settings, IStreamGroupService streamGroupService, ILogoService logoService) : ControllerBase
{
    [AllowAnonymous]
    [Route("{source}")]
    public async Task<IActionResult> GetLogo(string source, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(source) || source == "noimage.png" || source.EndsWithIgnoreCase("default.png"))
        {
            return Redirect($"{BuildInfo.PATH_BASE}/" + settings.CurrentValue.DefaultLogo);
        }

        (FileStream? fileStream, string? FileName, string? ContentType) = await logoService.GetLogoAsync(source, cancellationToken).ConfigureAwait(false);

        return fileStream == null ? Redirect($"{BuildInfo.PATH_BASE}/" + FileName ?? settings.CurrentValue.DefaultLogo) : File(fileStream, ContentType ?? "application/octet-stream", FileName);
    }

    [AllowAnonymous]
    [Route("[action]/{APIKey}")]
    [Route("[action]/{APIKey}/{isShort?}")]
    [Route("[action]/{APIKey}/{isShort?}/{encodedIds?}")]
    public async Task<ActionResult<Dictionary<int, SGFS>>> GetSMFS(
        string APIKey,
        string? encodedIds = null,
        bool? isShort = null,
        CancellationToken cancellationToken = default)
    {

        if (string.IsNullOrEmpty(APIKey) || !APIKey.Equals(settings.CurrentValue.APIKey))
        {
            logger.LogWarning("Invalid request: Invalid API Key.");
            return Unauthorized();
        }

        List<int>? sgProfileIds = null;

        if (!string.IsNullOrEmpty(encodedIds))
        {
            sgProfileIds = int.TryParse(encodedIds, out int testInt)
                ? [testInt]
                : encodedIds.Contains(',')
                      ? [.. encodedIds.Split(',').Select(int.Parse)]
                      : streamGroupService.DecodeProfileIds(encodedIds);

            if (sgProfileIds is null)
            {
                //logger.LogWarning("Invalid request: Missing required parameters.");
                return NotFound();
            }
        }

        Dictionary<int, SGFS> smfs = await streamGroupService.GetSMFS(sgProfileIds, isShort ?? false, cancellationToken);

        return smfs;
    }

    [AllowAnonymous]
    [Route("sm/{smChannelId}")]
    public async Task<IActionResult> GetSMChannelLogo(int smChannelId, CancellationToken cancellationToken)
    {
        string baseUrl = string.IsNullOrEmpty(BuildInfo.PATH_BASE) ? httpContextAccessor.GetUrl() : $"{BuildInfo.PATH_BASE}";

        if (smChannelId < 0)
        {
            return Redirect(baseUrl + settings.CurrentValue.DefaultLogo);
        }

        (FileStream? fileStream, string? FileName, string? ContentType) = await logoService.GetLogoForChannelAsync(smChannelId, cancellationToken).ConfigureAwait(false);

        return fileStream == null ? Redirect(FileName ?? baseUrl + settings.CurrentValue.DefaultLogo) : File(fileStream, ContentType ?? "application/octet-stream", FileName);
    }

    //Program ChannelLogo
    [AllowAnonymous]
    [Route("pr/{source}")]
    public async Task<IActionResult> GetProgramLogo(string source, CancellationToken cancellationToken)
    {
        //string baseUrl = httpContextAccessor.GetUrl();
        string baseUrl = string.IsNullOrEmpty(BuildInfo.PATH_BASE) ? httpContextAccessor.GetUrl() : $"{BuildInfo.PATH_BASE}";

        if (string.IsNullOrEmpty(source) || source == "noimage.png" || source.EndsWithIgnoreCase("default.png"))
        {
            return Redirect(baseUrl + settings.CurrentValue.DefaultLogo);
        }

        (FileStream? fileStream, string? FileName, string? ContentType) = await logoService.GetProgramLogoAsync(source, cancellationToken).ConfigureAwait(false);

        return fileStream == null ? Redirect(FileName ?? baseUrl + settings.CurrentValue.DefaultLogo) : File(fileStream, ContentType ?? "application/octet-stream", FileName);
    }

    //Custom ChannelLogo
    [AllowAnonymous]
    [Route("cu/{source}")]
    public async Task<IActionResult> GetCustomLogo(string source, CancellationToken cancellationToken)
    {
        //string baseUrl = httpContextAccessor.GetUrl();
        string baseUrl = string.IsNullOrEmpty(BuildInfo.PATH_BASE) ? httpContextAccessor.GetUrl() : $"{BuildInfo.PATH_BASE}";

        if (string.IsNullOrEmpty(source) || source == "noimage.png" || source.EndsWithIgnoreCase("default.png"))
        {
            return Redirect(baseUrl + settings.CurrentValue.DefaultLogo);
        }

        (FileStream? fileStream, string? FileName, string? ContentType) = await logoService.GetCustomLogoAsync(source, cancellationToken).ConfigureAwait(false);

        return fileStream == null ? Redirect(FileName ?? baseUrl + settings.CurrentValue.DefaultLogo) : File(fileStream, ContentType ?? "application/octet-stream", FileName);
    }

    //TVLogo
    [AllowAnonymous]
    [Route("tv/{source}")]
    public async Task<IActionResult> GetTvLogo(string source, CancellationToken cancellationToken)
    {
        //string baseUrl = httpContextAccessor.GetUrl();
        string baseUrl = string.IsNullOrEmpty(BuildInfo.PATH_BASE) ? httpContextAccessor.GetUrl() : $"{BuildInfo.PATH_BASE}";

        if (string.IsNullOrEmpty(source) || source == "noimage.png" || source.EndsWithIgnoreCase("default.png"))
        {
            return Redirect(baseUrl + settings.CurrentValue.DefaultLogo);
        }

        (FileStream? fileStream, string? FileName, string? ContentType) = await logoService.GetTVLogoAsync(source, cancellationToken).ConfigureAwait(false);

        return fileStream == null ? Redirect(FileName ?? baseUrl + settings.CurrentValue.DefaultLogo) : File(fileStream, ContentType ?? "application/octet-stream", FileName);
    }
}