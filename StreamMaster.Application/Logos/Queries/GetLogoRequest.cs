namespace StreamMaster.Application.Logos.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetLogoRequest(string Url) : IRequest<DataResponse<LogoDto?>>;

internal class GetLogoRequestHandler(ILogoService logoService)
    : IRequestHandler<GetLogoRequest, DataResponse<LogoDto?>>
{
    public async Task<DataResponse<LogoDto?>> Handle(GetLogoRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Url) || !request.Url.StartsWithIgnoreCase("/api/files"))
        {
            return DataResponse<LogoDto?>.NotFound;
        }

        if (!TryParseUrl(request.Url, out int id, out string? _))
        {
            return DataResponse<LogoDto?>.NotFound;
        }

        SMFileTypes filetype = id.GetSMFileTypEnumByValue();
        LogoDto? ret = await logoService.GetLogoFromCacheAsync(request.Url, filetype, cancellationToken).ConfigureAwait(false);
        return DataResponse<LogoDto?>.Success(ret);

    }

    /// <summary>
    /// Extracts the ID and filename from a URL with a constant prefix "/api/files/".
    /// </summary>
    /// <param name="url">The input URL.</param>
    /// <param name="id">The extracted ID as an integer.</param>
    /// <param name="filename">The extracted filename.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public static bool TryParseUrl(string url, out int id, out string? filename)
    {
        const string prefix = "/api/files/";
        id = 0;
        filename = null;

        if (string.IsNullOrWhiteSpace(url) || !url.StartsWith(prefix))
        {
            return false;
        }

        string remaining = url[prefix.Length..];
        string[] parts = remaining.Split('/', 2); // Split into at most 2 parts

        // Ensure we have both ID and filename
        if (parts.Length == 2 && int.TryParse(parts[0], out id))
        {
            filename = parts[1];
            return true;
        }

        return false;
    }
}