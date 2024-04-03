namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateM3UFileRequest(int? MaxStreamCount, int? StartingChannelNumber, bool? OverWriteChannels, List<string>? VODTags, bool? AutoUpdate, int? HoursToUpdate, int Id, string? Name, string? Url)
    : IRequest<DefaultAPIResponse>;

[LogExecutionTimeAspect]
public class UpdateM3UFileRequestHandler(IRepositoryWrapper Repository, IMapper Mapper, ISender Sender, IHubContext<StreamMasterHub, IStreamMasterHub> HubContext)
    : IRequestHandler<UpdateM3UFileRequest, DefaultAPIResponse>
{
    public async Task<DefaultAPIResponse> Handle(UpdateM3UFileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            List<FieldData> ret = [];
            M3UFile? m3uFile = await Repository.M3UFile.GetM3UFile(request.Id).ConfigureAwait(false);
            if (m3uFile == null)
            {
                return DefaultAPIResponse.NotFound;
            }

            bool needsUpdate = false;
            bool needsRefresh = false;

            if (request.VODTags != null)
            {
                needsUpdate = true;
                m3uFile.VODTags = request.VODTags;
                ret.Add(new FieldData("M3UFileDto", m3uFile.Id.ToString(), "vodTags", m3uFile.VODTags));
            }


            if (!string.IsNullOrEmpty(request.Url) && m3uFile.Url != request.Url)
            {
                needsUpdate = true;
                m3uFile.Url = request.Url;
                ret.Add(new FieldData("M3UFileDto", m3uFile.Id.ToString(), "url", m3uFile.Url));
            }

            if (request.OverWriteChannels != null)
            {
                needsUpdate = true;
                m3uFile.OverwriteChannelNumbers = (bool)request.OverWriteChannels;
                ret.Add(new FieldData("M3UFileDto", m3uFile.Id.ToString(), "overwriteChannelNumbers", m3uFile.OverwriteChannelNumbers));
            }

            if (!string.IsNullOrEmpty(request.Name) && m3uFile.Name != request.Name)
            {
                m3uFile.Name = request.Name;
                ret.Add(new FieldData("M3UFileDto", m3uFile.Id.ToString(), "name", m3uFile.Name));
            }

            if (request.MaxStreamCount != null && m3uFile.MaxStreamCount != request.MaxStreamCount)
            {
                m3uFile.MaxStreamCount = (int)request.MaxStreamCount;
                ret.Add(new FieldData("M3UFileDto", m3uFile.Id.ToString(), "maxStreamCount", m3uFile.MaxStreamCount));
            }

            if (request.AutoUpdate != null && m3uFile.AutoUpdate != request.AutoUpdate)
            {
                m3uFile.AutoUpdate = (bool)request.AutoUpdate;
                ret.Add(new FieldData("M3UFileDto", m3uFile.Id.ToString(), "autoUpdate", m3uFile.AutoUpdate));
            }

            if (request.StartingChannelNumber != null && m3uFile.StartingChannelNumber != request.StartingChannelNumber)
            {
                m3uFile.StartingChannelNumber = (int)request.StartingChannelNumber;
                ret.Add(new FieldData("M3UFileDto", m3uFile.Id.ToString(), "startingChannelNumber", m3uFile.StartingChannelNumber));
            }

            if (request.HoursToUpdate != null && m3uFile.HoursToUpdate != request.HoursToUpdate)
            {
                m3uFile.HoursToUpdate = (int)request.HoursToUpdate;
                ret.Add(new FieldData("M3UFileDto", m3uFile.Id.ToString(), "hoursToUpdate", m3uFile.HoursToUpdate));
            }

            Repository.M3UFile.UpdateM3UFile(m3uFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);

            if (needsRefresh)
            {
                await Sender.Send(new RefreshM3UFileRequest(m3uFile.Id, true), cancellationToken).ConfigureAwait(false);
            }


            if (needsUpdate)
            {
                await Sender.Send(new ProcessM3UFileRequest(m3uFile.Id), cancellationToken).ConfigureAwait(false);
            }


            if (ret.Count > 0)
            {
                await HubContext.Clients.All.SetField(ret).ConfigureAwait(false);
            }

            return DefaultAPIResponse.Ok;
        }
        catch (Exception ex)
        {
            return DefaultAPIResponse.ErrorWithMessage($"Failed M3U update : {ex}");
        }

    }
}
