namespace StreamMaster.Application.EPGFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateEPGFileRequest(int? EPGNumber, string? Color, int? TimeShift, bool? AutoUpdate, int? HoursToUpdate, int Id, string? Name, string? Url)
    : IRequest<APIResponse>;

public class UpdateEPGFileRequestHandler(ICacheManager cacheManager, IDataRefreshService dataRefreshService, IJobStatusService jobStatusService, IRepositoryWrapper Repository)
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
            bool isColorChanged = false;
            bool isNameChanged = false;
            int? oldEPGNumber = null;

            if (request.EPGNumber.HasValue)
            {
                if (!Repository.EPGFile.GetQuery(x => x.EPGNumber == request.EPGNumber.Value).Any())
                {
                    cacheManager.ClearEPGDataByEPGNumber(epgFile.EPGNumber);
                    oldEPGNumber = epgFile.EPGNumber;
                    epgFile.EPGNumber = request.EPGNumber.Value;
                    ret.Add(new FieldData(() => epgFile.EPGNumber));
                }
            }

            if (request.Url != null && epgFile.Url != request.Url)
            {
                epgFile.Url = request.Url?.Length == 0 ? null : request.Url;
                ret.Add(new FieldData(() => epgFile.Url ?? ""));
            }

            if (request.Color != null && epgFile.Color != request.Color)
            {
                isColorChanged = true;
                epgFile.Color = request.Color;
                ret.Add(new FieldData(() => epgFile.Color));
            }

            if (request.TimeShift.HasValue)
            {
                epgFile.TimeShift = request.TimeShift.Value;
                ret.Add(new FieldData(() => epgFile.TimeShift));
            }

            if (!string.IsNullOrEmpty(request.Name) && epgFile.Name != request.Name)
            {
                isNameChanged = true;
                epgFile.Name = request.Name;
                ret.Add(new FieldData(() => epgFile.Name));
            }

            if (request.AutoUpdate.HasValue && epgFile.AutoUpdate != request.AutoUpdate)
            {
                epgFile.AutoUpdate = request.AutoUpdate.Value;
                ret.Add(new FieldData(() => epgFile.AutoUpdate));
            }

            if (request.HoursToUpdate.HasValue && epgFile.HoursToUpdate != request.HoursToUpdate)
            {
                epgFile.HoursToUpdate = request.HoursToUpdate.Value;
                ret.Add(new FieldData(() => epgFile.HoursToUpdate));
            }

            Repository.EPGFile.UpdateEPGFile(epgFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);
            epgFile.WriteJSON();

            if (isNameChanged)
            {
                await dataRefreshService.RefreshAllEPG();
            }
            else
            {
                if (ret.Count > 0)
                {
                    await dataRefreshService.SetField(ret).ConfigureAwait(false);
                }
                if (isColorChanged)
                {
                    await dataRefreshService.RefreshEPGColors();
                    await dataRefreshService.RefreshAllEPG();
                }
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