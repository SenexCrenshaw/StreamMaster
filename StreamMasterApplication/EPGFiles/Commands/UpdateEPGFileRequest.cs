using FluentValidation;

using StreamMasterApplication.M3UFiles.Commands;
using StreamMasterApplication.Programmes.Queries;

using StreamMasterDomain.EPG;

namespace StreamMasterApplication.EPGFiles.Commands;

public class UpdateEPGFileRequest : BaseFileRequest, IRequest<EPGFileDto?>
{
    public int? EPGRank { get; set; }
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
                List<Programme> programmes = await Sender.Send(new GetProgrammesRequest(), cancellationToken).ConfigureAwait(false);
                int c = programmes.Count;
                _ = programmes.RemoveAll(a => a.EPGFileId == epgFile.Id);
                int d = programmes.Count;
                MemoryCache.SetCache(programmes);

                List<ChannelLogoDto> channelLogos = MemoryCache.ChannelLogos();
                _ = channelLogos.RemoveAll(a => a.EPGFileId == epgFile.Id);
                MemoryCache.SetCache(channelLogos);

                List<IconFileDto> programmeIcons = MemoryCache.ProgrammeIcons();
                _ = programmeIcons.RemoveAll(a => a.FileId == epgFile.Id);
                MemoryCache.SetProgrammeLogos(programmeIcons);

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