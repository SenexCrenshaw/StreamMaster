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

        if (!request.Url.TryParseUrl(out int _, out string? _))
        {
            return DataResponse<LogoDto?>.NotFound;
        }

        (FileStream? fileStream, string? FileName, string? ContentType) = await logoService.GetLogoAsync(request.Url, cancellationToken).ConfigureAwait(false);

        if (fileStream == null)
        {
            return DataResponse<LogoDto?>.NotFound;
        }

        byte[] allBytes = await fileStream.GetStreamBytes(cancellationToken);
        LogoDto ret = new(request.Url, ContentType ?? "application/octet-stream", FileName ?? "", allBytes);
        return DataResponse<LogoDto?>.Success(ret);
    }
}