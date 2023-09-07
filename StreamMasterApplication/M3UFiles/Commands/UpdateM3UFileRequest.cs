using FluentValidation;

namespace StreamMasterApplication.M3UFiles.Commands;

public class UpdateM3UFileRequest : BaseFileRequest, IRequest<M3UFile?>
{
    public int? MaxStreamCount { get; set; }
    public int? StartingChannelNumber { get; set; }
}

public class UpdateM3UFileRequestValidator : AbstractValidator<UpdateM3UFileRequest>
{
    public UpdateM3UFileRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().GreaterThanOrEqualTo(0);
    }
}

public class UpdateM3UFileRequestHandler : BaseMemoryRequestHandler, IRequestHandler<UpdateM3UFileRequest, M3UFile?>
{

    public UpdateM3UFileRequestHandler(ILogger<UpdateM3UFileRequest> logger, IRepositoryWrapper repository, IMapper mapper, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache)
: base(logger, repository, mapper, publisher, sender, hubContext, memoryCache) { }

    public async Task<M3UFile?> Handle(UpdateM3UFileRequest command, CancellationToken cancellationToken)
    {
        try
        {

            M3UFile? m3uFile = await Repository.M3UFile.GetM3UFileByIdAsync(command.Id).ConfigureAwait(false);
            if (m3uFile == null)
            {
                return null;
            }

            bool isChanged = false;

            if (!string.IsNullOrEmpty(command.Description) && m3uFile.Description != command.Description)
            {
                isChanged = true;
                m3uFile.Description = command.Description;
            }

            if (!string.IsNullOrEmpty(command.Url) && m3uFile.Url != command.Url)
            {
                isChanged = true;
                m3uFile.Url = command.Url;
            }

            if (!string.IsNullOrEmpty(command.Name) && m3uFile.Name != command.Name)
            {
                isChanged = true;
                m3uFile.Name = command.Name;
            }

            if (command.MaxStreamCount != null && m3uFile.MaxStreamCount != command.MaxStreamCount)
            {
                isChanged = true;
                m3uFile.MaxStreamCount = (int)command.MaxStreamCount;
            }

            if (command.AutoUpdate != null && m3uFile.AutoUpdate != command.AutoUpdate)
            {
                isChanged = true;
                m3uFile.AutoUpdate = (bool)command.AutoUpdate;
            }

            if (command.StartingChannelNumber != null && m3uFile.StartingChannelNumber != command.StartingChannelNumber)
            {
                isChanged = true;
                m3uFile.StartingChannelNumber = (int)command.StartingChannelNumber;
            }

            if (command.HoursToUpdate != null && m3uFile.HoursToUpdate != command.HoursToUpdate)
            {
                isChanged = true;
                m3uFile.HoursToUpdate = (int)command.HoursToUpdate;
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
