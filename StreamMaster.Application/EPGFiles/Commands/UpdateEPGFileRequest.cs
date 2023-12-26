using FluentValidation;

using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Models;
using StreamMaster.Domain.Repository;
using StreamMaster.Domain.Services;

using StreamMaster.Application.M3UFiles.Commands;

namespace StreamMaster.Application.EPGFiles.Commands;

public class UpdateEPGFileRequest : BaseFileRequest, IRequest<EPGFileDto?>
{
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

            if (!string.IsNullOrEmpty(request.Description) && epgFile.Description != request.Description)
            {
                isChanged = true;
                epgFile.Description = request.Description;
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
            epgFile.WriteJSON();
            EPGFileDto ret = Mapper.Map<EPGFileDto>(epgFile);

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