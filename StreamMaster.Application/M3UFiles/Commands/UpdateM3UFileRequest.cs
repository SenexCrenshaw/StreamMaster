using StreamMaster.Application.Services;

namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateM3UFileRequest(int? MaxStreamCount, int? StartingChannelNumber, bool? OverWriteChannels, List<string>? VODTags, bool? AutoUpdate, int? HoursToUpdate, int Id, string? Name, string? Url)
    : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class UpdateM3UFileRequestHandler(IRepositoryWrapper Repository, IBackgroundTaskQueue taskQueue, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<UpdateM3UFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<FieldData> ret = [];
            M3UFile? m3uFile = await Repository.M3UFile.GetM3UFile(request.Id).ConfigureAwait(false);
            if (m3uFile == null)
            {
                return APIResponse.NotFound;
            }

            bool needsUpdate = false;
            bool needsRefresh = false;

            if (request.VODTags != null)
            {
                m3uFile.VODTags = request.VODTags;
                ret.Add(new FieldData(M3UFile.MainGet, m3uFile.Id.ToString(), "VODTags", m3uFile.VODTags));
            }


            if (m3uFile.Url != request.Url)
            {
                needsRefresh = true;
                m3uFile.Url = request.Url;
                ret.Add(new FieldData(M3UFile.MainGet, m3uFile.Id.ToString(), "Url", m3uFile.Url));
            }

            if (request.OverWriteChannels != null)
            {
                needsUpdate = true;
                m3uFile.OverwriteChannelNumbers = (bool)request.OverWriteChannels;
                ret.Add(new FieldData(M3UFile.MainGet, m3uFile.Id.ToString(), "OverwriteChannelNumbers", m3uFile.OverwriteChannelNumbers));
            }

            if (!string.IsNullOrEmpty(request.Name))
            {

                m3uFile.Name = request.Name;
                ret.Add(new FieldData(M3UFile.MainGet, m3uFile.Id.ToString(), "Name", m3uFile.Name));
            }

            if (request.MaxStreamCount.HasValue)
            {
                m3uFile.MaxStreamCount = (int)request.MaxStreamCount;
                ret.Add(new FieldData(M3UFile.MainGet, m3uFile.Id.ToString(), "MaxStreamCount", m3uFile.MaxStreamCount));
            }

            if (request.AutoUpdate != null)
            {
                m3uFile.AutoUpdate = (bool)request.AutoUpdate;
                ret.Add(new FieldData(M3UFile.MainGet, m3uFile.Id.ToString(), "AutoUpdate", m3uFile.AutoUpdate));
            }

            if (request.StartingChannelNumber.HasValue)
            {
                needsUpdate = true;
                m3uFile.StartingChannelNumber = request.StartingChannelNumber.Value;
                ret.Add(new FieldData(M3UFile.MainGet, m3uFile.Id.ToString(), "StartingChannelNumber", m3uFile.StartingChannelNumber));
            }


            if (request.HoursToUpdate.HasValue)
            {
                m3uFile.HoursToUpdate = request.HoursToUpdate.Value;
                ret.Add(new FieldData(M3UFile.MainGet, m3uFile.Id.ToString(), "HoursToUpdate", m3uFile.HoursToUpdate));
            }

            Repository.M3UFile.UpdateM3UFile(m3uFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);

            if (needsRefresh)
            {
                await Sender.Send(new RefreshM3UFileRequest(m3uFile.Id, true), cancellationToken).ConfigureAwait(false);
            }

            if (needsUpdate)
            {
                ProcessM3UFileRequest processM3UFileRequest = new(m3uFile.Id);
                await taskQueue.ProcessM3UFile(processM3UFileRequest, cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            if (ret.Count > 0)
            {
                await HubContext.Clients.All.SetField(ret).ConfigureAwait(false);
            }

            return APIResponse.Success;
        }
        catch (Exception ex)
        {
            return APIResponse.ErrorWithMessage(ex, $"Failed M3U update");
        }

    }



}
