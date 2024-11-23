namespace StreamMaster.Application.Logos.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetLogoForChannelRequest(int SMChannelId) : IRequest<DataResponse<LogoDto?>>;

internal class GetLogoForChannelRequestHandler(ILogoService logoService, IRepositoryWrapper repositoryWrapper)
    : IRequestHandler<GetLogoForChannelRequest, DataResponse<LogoDto?>>
{
    public async Task<DataResponse<LogoDto?>> Handle(GetLogoForChannelRequest request, CancellationToken cancellationToken)
    {
        SMChannel? channel = repositoryWrapper.SMChannel.GetSMChannel(request.SMChannelId);
        if (channel == null)
        {
            return DataResponse<LogoDto?>.NotFound;
        }

        if (!channel.Logo.TryParseUrl(out int id, out string? _))
        {
            return DataResponse<LogoDto?>.NotFound;
        }

        SMFileTypes filetype = id.GetSMFileTypEnumByValue();
        LogoDto? ret = await logoService.GetLogoFromCacheAsync(channel.Logo, filetype, cancellationToken).ConfigureAwait(false);
        return DataResponse<LogoDto?>.Success(ret);
    }
}