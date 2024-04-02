namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateM3UFileRequest(int? MaxStreamCount, int? StartingChannelNumber, bool? OverWriteChannels, List<string>? VODTags, bool? AutoUpdate, int? HoursToUpdate, int Id, string? Name, string? Url)
    : IRequest<DefaultAPIResponse>;

[LogExecutionTimeAspect]
public class UpdateM3UFileRequestHandler(IRepositoryWrapper Repository, IMapper Mapper, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<UpdateM3UFileRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(UpdateM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            M3UFile? m3uFile = await Repository.M3UFile.GetM3UFile(request.Id).ConfigureAwait(false);
            if (m3uFile == null)
            {
                return DefaultAPIResponse.NotFound;
            }

            bool isChanged = false;
            bool needsUpdate = false;
            bool needsRefresh = false;

            if (request.VODTags != null)
            {
                isChanged = true;
                needsUpdate = true;
                m3uFile.VODTags = request.VODTags;
            }


            if (!string.IsNullOrEmpty(request.Url) && m3uFile.Url != request.Url)
            {
                isChanged = true;
                needsUpdate = true;
                m3uFile.Url = request.Url;
            }

            if (request.OverWriteChannels != null)
            {
                isChanged = true;
                needsUpdate = true;
                m3uFile.OverwriteChannelNumbers = (bool)request.OverWriteChannels;
            }

            if (!string.IsNullOrEmpty(request.Name) && m3uFile.Name != request.Name)
            {
                isChanged = true;
                m3uFile.Name = request.Name;
            }

            if (request.MaxStreamCount != null && m3uFile.MaxStreamCount != request.MaxStreamCount)
            {
                isChanged = true;
                m3uFile.MaxStreamCount = (int)request.MaxStreamCount;
            }

            if (request.AutoUpdate != null && m3uFile.AutoUpdate != request.AutoUpdate)
            {
                isChanged = true;
                m3uFile.AutoUpdate = (bool)request.AutoUpdate;
            }

            if (request.StartingChannelNumber != null && m3uFile.StartingChannelNumber != request.StartingChannelNumber)
            {
                isChanged = true;
                m3uFile.StartingChannelNumber = (int)request.StartingChannelNumber;
            }

            if (request.HoursToUpdate != null && m3uFile.HoursToUpdate != request.HoursToUpdate)
            {
                isChanged = true;
                m3uFile.HoursToUpdate = (int)request.HoursToUpdate;
            }

            Repository.M3UFile.UpdateM3UFile(m3uFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);

            if (needsRefresh)
            {

                await Sender.Send(new RefreshM3UFileRequest(m3uFile.Id, true), cancellationToken).ConfigureAwait(false);
            }


            if (needsUpdate)
            {
                await Sender.Send(new ProcessM3UFileRequest(m3uFile.Id), cancellationToken).ConfigureAwait(false);
            }

            M3UFileDto ret = Mapper.Map<M3UFileDto>(m3uFile);
            if (isChanged)
            {
                await HubContext.Clients.All.M3UFilesRefresh().ConfigureAwait(false);
            }

            return DefaultAPIResponse.Ok;
        }
        catch (Exception ex)
        {
            return DefaultAPIResponse.ErrorWithMessage($"Failed M3U update : {ex}");
        }

    }
}
