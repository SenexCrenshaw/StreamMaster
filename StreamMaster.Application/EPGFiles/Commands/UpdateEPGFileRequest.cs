using FluentValidation;

using StreamMaster.Application.M3UFiles.Commands;
using StreamMaster.Application.VideoStreams.Commands;

namespace StreamMaster.Application.EPGFiles.Commands;

public class UpdateEPGFileRequest : BaseFileRequest, IRequest<EPGFileDto?>
{
    public int? EPGNumber { get; set; }
    public string? Color { get; set; }
}

public class UpdateEPGFileRequestValidator : AbstractValidator<UpdateEPGFileRequest>
{
    public UpdateEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().GreaterThanOrEqualTo(0);
    }
}

public class UpdateEPGFileRequestHandler(ILogger<UpdateEPGFileRequest> logger, IRepositoryWrapper repository, IMapper mapper, ISettingsService settingsService, IPublisher publisher, ISender sender, IHubContext<StreamMasterHub, IStreamMasterHub> hubContext, IMemoryCache memoryCache) : BaseMediatorRequestHandler(logger, repository, mapper, settingsService, publisher, sender, hubContext, memoryCache), IRequestHandler<UpdateEPGFileRequest, EPGFileDto?>
{
    public async Task<EPGFileDto?> Handle(UpdateEPGFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            EPGFile? epgFile = await Repository.EPGFile.GetEPGFileById(request.Id).ConfigureAwait(false);

            if (epgFile == null)
            {
                return null;
            }

            bool isChanged = false;
            bool isNameChanged = false;
            int? oldEPGNumber = null;

            if (!string.IsNullOrEmpty(request.Description) && epgFile.Description != request.Description)
            {
                isChanged = true;
                epgFile.Description = request.Description;
            }

            if (request.EPGNumber != null && request.EPGNumber >= 0 && epgFile.EPGNumber != request.EPGNumber)
            {
                isChanged = true;
                if (!Repository.EPGFile.FindByCondition(x => x.EPGNumber == request.EPGNumber).Any())
                {
                    oldEPGNumber = epgFile.EPGNumber;
                    epgFile.EPGNumber = (int)request.EPGNumber;
                }
                else
                {

                }
            }


            if (request.Url != null && epgFile.Url != request.Url)
            {
                isChanged = true;
                epgFile.Url = request.Url == "" ? null : request.Url;
            }

            if (request.Color != null && epgFile.Color != request.Color)
            {
                isChanged = true;
                epgFile.Color = request.Color;
            }

            if (!string.IsNullOrEmpty(request.Name) && epgFile.Name != request.Name)
            {
                isChanged = true;
                isNameChanged = true;
                epgFile.Name = request.Name;
            }

            if (request.AutoUpdate != null && epgFile.AutoUpdate != request.AutoUpdate)
            {
                isChanged = true;
                epgFile.AutoUpdate = (bool)request.AutoUpdate;
            }

            if (request.HoursToUpdate != null && epgFile.HoursToUpdate != request.HoursToUpdate)
            {
                isChanged = true;
                epgFile.HoursToUpdate = (int)request.HoursToUpdate;
            }

            Repository.EPGFile.UpdateEPGFile(epgFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);
            epgFile.WriteJSON(logger);
            EPGFileDto ret = Mapper.Map<EPGFileDto>(epgFile);

            if (oldEPGNumber != null && request.EPGNumber != null)
            {
                await Sender.Send(new VideoStreamChangeEPGNumberRequest((int)oldEPGNumber, (int)request.EPGNumber), cancellationToken).ConfigureAwait(false);
            }

            if (isNameChanged)
            {
                await Publisher.Publish(new EPGFileAddedEvent(ret), cancellationToken).ConfigureAwait(false);
            }

            if (isChanged)
            {
                await HubContext.Clients.All.EPGFilesRefresh().ConfigureAwait(false);
            }

            return ret;
        }
        catch (Exception)
        {
        }
        return null;
    }
}