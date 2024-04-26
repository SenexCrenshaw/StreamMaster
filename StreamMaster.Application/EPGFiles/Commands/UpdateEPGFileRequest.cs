using StreamMaster.Application.VideoStreams.Commands;

namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateEPGFileRequest(int? EPGNumber, string? Color, int? TimeShift, bool? AutoUpdate, int? HoursToUpdate, int Id, string? Name, string? Url)
    : IRequest<APIResponse>;


public class UpdateEPGFileRequestHandler(ILogger<UpdateEPGFileRequest> logger, IJobStatusService jobStatusService, IRepositoryWrapper Repository, IMapper Mapper, IPublisher Publisher, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<UpdateEPGFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateEPGFileRequest request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManagerUpdateEPG(request.Id);
        try
        {

            if (jobManager.IsRunning)
            {
                return APIResponse.NotFound;
            }


            List<FieldData> ret = [];

            EPGFile? epgFile = await Repository.EPGFile.GetEPGFileById(request.Id).ConfigureAwait(false);

            if (epgFile == null)
            {
                return APIResponse.NotFound;
            }
            jobManager.Start();

            bool isChanged = false;
            bool isColorChanged = false;
            bool isNameChanged = false;
            int? oldEPGNumber = null;

            if (request.EPGNumber.HasValue)
            {
                isChanged = true;
                if (!Repository.EPGFile.GetQuery(x => x.EPGNumber == request.EPGNumber.Value).Any())
                {
                    oldEPGNumber = epgFile.EPGNumber;
                    epgFile.EPGNumber = request.EPGNumber.Value;
                    ret.Add(new FieldData(() => epgFile.EPGNumber));
                }
            }

            if (request.Url != null && epgFile.Url != request.Url)
            {
                isChanged = true;
                epgFile.Url = request.Url == "" ? null : request.Url;
                ret.Add(new FieldData(() => epgFile.Url));
            }

            if (request.Color != null && epgFile.Color != request.Color)
            {
                isColorChanged = true;
                epgFile.Color = request.Color;
                ret.Add(new FieldData(() => epgFile.Color));
            }

            if (request.TimeShift.HasValue)
            {
                isChanged = true;
                epgFile.TimeShift = request.TimeShift.Value;
                ret.Add(new FieldData(() => epgFile.TimeShift));
            }

            if (!string.IsNullOrEmpty(request.Name) && epgFile.Name != request.Name)
            {
                isChanged = true;
                isNameChanged = true;
                epgFile.Name = request.Name;
                ret.Add(new FieldData(() => epgFile.Name));
            }

            if (request.AutoUpdate != null && epgFile.AutoUpdate != request.AutoUpdate)
            {
                isChanged = true;
                epgFile.AutoUpdate = (bool)request.AutoUpdate;
                ret.Add(new FieldData(() => epgFile.AutoUpdate));
            }

            if (request.HoursToUpdate != null && epgFile.HoursToUpdate != request.HoursToUpdate)
            {
                isChanged = true;
                epgFile.HoursToUpdate = (int)request.HoursToUpdate;
                ret.Add(new FieldData(() => epgFile.HoursToUpdate));
            }

            Repository.EPGFile.UpdateEPGFile(epgFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);
            epgFile.WriteJSON(logger);

            if (oldEPGNumber != null && request.EPGNumber != null)
            {
                await Sender.Send(new VideoStreamChangeEPGNumberRequest((int)oldEPGNumber, (int)request.EPGNumber), cancellationToken).ConfigureAwait(false);
            }

            if (isNameChanged)
            {
                await Publisher.Publish(new EPGFileAddedEvent(Mapper.Map<EPGFileDto>(epgFile)), cancellationToken).ConfigureAwait(false);
            }

            if (isColorChanged)
            {
                //await HubContext.Clients.All.EPGFilesRefresh().ConfigureAwait(false);
                //if ( )
                await HubContext.Clients.All.DataRefresh("GetEPGColors");
            }

            if (ret.Count > 0)
            {
                await HubContext.Clients.All.SetField(ret).ConfigureAwait(false);
            }

            jobManager.SetSuccessful();
            return APIResponse.Success;
        }
        catch (Exception ex)
        {
            jobManager.SetError();
            return APIResponse.ErrorWithMessage(ex, "Refresh EPG failed");
        }

    }
}