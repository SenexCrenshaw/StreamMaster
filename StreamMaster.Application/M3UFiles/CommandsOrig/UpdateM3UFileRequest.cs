namespace StreamMaster.Application.M3UFiles.Commands;

public class UpdateM3UFileRequest : BaseFileRequest, IRequest<M3UFile?>
{
    //public int? StreamURLPrefixInt { get; set; }
    public int? MaxStreamCount { get; set; }
    public int? StartingChannelNumber { get; set; }
    public bool? OverWriteChannels { get; set; }
    public List<string>? VODTags { get; set; }
}


[LogExecutionTimeAspect]
public class UpdateM3UFileRequestHandler(ILogger<UpdateM3UFileRequest> logger, IRepositoryWrapper Repository, IMapper Mapper, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<UpdateM3UFileRequest, M3UFile?>
{
    public async Task<M3UFile?> Handle(UpdateM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {

            M3UFile? m3uFile = await Repository.M3UFile.GetM3UFile(request.Id).ConfigureAwait(false);
            if (m3uFile == null)
            {
                return null;
            }

            bool isChanged = false;
            bool needsUpdate = false;
            bool needsRefresh = false;

            if (!string.IsNullOrEmpty(request.Description) && m3uFile.Description != request.Description)
            {
                isChanged = true;
                m3uFile.Description = request.Description;
            }

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
                await Sender.Send(new ProcessM3UFileRequest(m3uFile.Id, request.OverWriteChannels), cancellationToken).ConfigureAwait(false);
            }

            M3UFileDto ret = Mapper.Map<M3UFileDto>(m3uFile);
            if (isChanged)
            {
                await HubContext.Clients.All.M3UFilesRefresh().ConfigureAwait(false);
            }

            return m3uFile;
        }
        catch (Exception)
        {
        }
        return null;
    }
}
