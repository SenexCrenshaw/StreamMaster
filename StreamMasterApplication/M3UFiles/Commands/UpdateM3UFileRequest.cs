namespace StreamMasterApplication.M3UFiles.Commands;

public class UpdateM3UFileRequest : BaseFileRequest, IRequest<M3UFile?>
{
    //public int? StreamURLPrefixInt { get; set; }
    public int? MaxStreamCount { get; set; }
    public int? StartingChannelNumber { get; set; }
}


[LogExecutionTimeAspect]
public class UpdateM3UFileRequestHandler(ILogger<UpdateM3UFileRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateM3UFileRequest, M3UFile?>
{
    public async Task<M3UFile?> Handle(UpdateM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {

            M3UFile? m3uFile = await Repository.M3UFile.GetM3UFileById(request.Id).ConfigureAwait(false);
            if (m3uFile == null)
            {
                return null;
            }

            bool isChanged = false;

            if (!string.IsNullOrEmpty(request.Description) && m3uFile.Description != request.Description)
            {
                isChanged = true;
                m3uFile.Description = request.Description;
            }

            //if (request.StreamURLPrefixInt != null && m3uFile.StreamURLPrefix != (M3UFileStreamURLPrefix)request.StreamURLPrefixInt)
            //{
            //    isChanged = true;
            //    m3uFile.StreamURLPrefix = (M3UFileStreamURLPrefix)request.StreamURLPrefixInt;
            //}

            if (!string.IsNullOrEmpty(request.Url) && m3uFile.Url != request.Url)
            {
                isChanged = true;
                m3uFile.Url = request.Url;
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
            m3uFile.WriteJSON();

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
