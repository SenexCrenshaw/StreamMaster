using FluentValidation;

using StreamMaster.Application.VideoStreams.Commands;

namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateEPGFileRequest(int? EPGNumber, string? Color, int? TimeShift, bool? AutoUpdate, int? HoursToUpdate, int Id, string? Name, string? Url) : IRequest<EPGFileDto?>;

public class UpdateEPGFileRequestValidator : AbstractValidator<UpdateEPGFileRequest>
{
    public UpdateEPGFileRequestValidator()
    {
        _ = RuleFor(v => v.Id).NotNull().GreaterThanOrEqualTo(0);
    }
}

public class UpdateEPGFileRequestHandler(ILogger<UpdateEPGFileRequest> logger, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<UpdateEPGFileRequest, EPGFileDto?>
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

            if (request.EPGNumber.HasValue)
            {
                isChanged = true;
                if (!Repository.EPGFile.GetQuery(x => x.EPGNumber == request.EPGNumber.Value).Any())
                {
                    oldEPGNumber = epgFile.EPGNumber;
                    epgFile.EPGNumber = request.EPGNumber.Value;
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

            if (request.TimeShift.HasValue)
            {
                isChanged = true;
                epgFile.TimeShift = request.TimeShift.Value;
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