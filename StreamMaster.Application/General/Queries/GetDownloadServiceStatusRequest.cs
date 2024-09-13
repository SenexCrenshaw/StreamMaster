namespace StreamMaster.Application.General.Queries;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record GetDownloadServiceStatusRequest : IRequest<DataResponse<ImageDownloadServiceStatus>>;

internal class GetDownloadServiceStatusRequestHandler(IImageDownloadService imageDownloadService) : IRequestHandler<GetDownloadServiceStatusRequest, DataResponse<ImageDownloadServiceStatus>>
{
    public Task<DataResponse<ImageDownloadServiceStatus>> Handle(GetDownloadServiceStatusRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(DataResponse<ImageDownloadServiceStatus>.Success(imageDownloadService.imageDownloadServiceStatus));
    }
}
