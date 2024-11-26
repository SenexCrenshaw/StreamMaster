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

        (FileStream? fileStream, string? FileName, string? ContentType) = await logoService.GetLogoAsync(channel.Logo, cancellationToken).ConfigureAwait(false);

        if (fileStream == null)
        {
            return DataResponse<LogoDto?>.NotFound;
        }

        byte[] allBytes = await fileStream.GetStreamBytes(cancellationToken);
        LogoDto ret = new(channel.Logo, ContentType ?? "application/octet-stream", FileName ?? "", allBytes);
        return DataResponse<LogoDto?>.Success(ret);
    }
}