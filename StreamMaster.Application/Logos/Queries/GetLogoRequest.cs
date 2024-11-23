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

        if (!request.Url.TryParseUrl(out int id, out string? _))
        {
            return DataResponse<LogoDto?>.NotFound;
        }

        SMFileTypes filetype = id.GetSMFileTypEnumByValue();
        LogoDto? ret = await logoService.GetLogoFromCacheAsync(request.Url, filetype, cancellationToken).ConfigureAwait(false);
        return DataResponse<LogoDto?>.Success(ret);
    }
}