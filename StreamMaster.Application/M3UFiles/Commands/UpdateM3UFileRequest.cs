using StreamMaster.Application.Services;
using StreamMaster.Application.StreamGroupSMChannelLinks.Commands;

namespace StreamMaster.Application.M3UFiles.Commands;

[SMAPI]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public record UpdateM3UFileRequest(int? MaxStreamCount, int? StartingChannelNumber, string? DefaultStreamGroupName, bool? SyncChannels, List<string>? VODTags, bool? AutoSetChannelNumbers, bool? AutoUpdate, int? HoursToUpdate, int Id, string? Name, string? Url)
    : IRequest<APIResponse>;

[LogExecutionTimeAspect]
public class UpdateM3UFileRequestHandler(IRepositoryWrapper Repository, IRepositoryContext repositoryContext, ICacheManager CacheManager, IBackgroundTaskQueue taskQueue, IJobStatusService jobStatusService, ISender Sender, IDataRefreshService dataRefreshService)
    : IRequestHandler<UpdateM3UFileRequest, APIResponse>
{
    public async Task<APIResponse> Handle(UpdateM3UFileRequest request, CancellationToken cancellationToken)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManagerUpdateM3U(request.Id);

        try
        {
            if (jobManager.IsRunning)
            {
                return APIResponse.NotFound;
            }

            List<FieldData> ret = [];
            M3UFile? m3uFile = await Repository.M3UFile.GetM3UFileAsync(request.Id).ConfigureAwait(false);
            if (m3uFile == null)
            {
                return APIResponse.NotFound;
            }
            jobManager.Start();

            bool needsUpdate = false;
            bool needsRefresh = false;
            bool nameChange = false;
            bool defaultNameChange = false;
            int oldSGId = 0;

            if (request.StartingChannelNumber.HasValue && m3uFile.StartingChannelNumber != request.StartingChannelNumber.Value)
            {
                m3uFile.StartingChannelNumber = request.StartingChannelNumber.Value;
                ret.Add(new FieldData(() => m3uFile.StartingChannelNumber));
            }

            if (request.AutoSetChannelNumbers.HasValue)
            {
                m3uFile.AutoSetChannelNumbers = request.AutoSetChannelNumbers.Value;
                ret.Add(new FieldData(() => m3uFile.AutoSetChannelNumbers));
            }

            if (request.AutoSetChannelNumbers.HasValue)
            {
                if (m3uFile.AutoSetChannelNumbers)
                {
                    string sql = $"UPDATE public.\"SMStreams\" SET \"ChannelNumber\"=\"FilePosition\"+{m3uFile.StartingChannelNumber} WHERE \"M3UFileId\"={m3uFile.Id};";
                    await repositoryContext.ExecuteSqlRawAsync(sql, cancellationToken);
                    await dataRefreshService.RefreshSMStreams().ConfigureAwait(false);
                }
                else
                {
                    needsUpdate = true;
                }
            }

            if (request.VODTags != null)
            {
                m3uFile.VODTags = request.VODTags;

                ret.Add(new FieldData(() => m3uFile.VODTags));
            }

            if (!string.IsNullOrEmpty(request.Url) && m3uFile.Url != request.Url)
            {
                needsRefresh = true;
                m3uFile.Url = request.Url;
                ret.Add(new FieldData(() => m3uFile.Url));
            }

            if (!string.IsNullOrEmpty(request.Name) && request.Name != m3uFile.Name)
            {
                nameChange = true;
                m3uFile.Name = request.Name;
                ret.Add(new FieldData(() => m3uFile.Name));
            }

            if (!string.IsNullOrEmpty(request.DefaultStreamGroupName) && request.DefaultStreamGroupName != m3uFile.DefaultStreamGroupName)
            {
                if (m3uFile.DefaultStreamGroupName != null)
                {
                    StreamGroupDto? sg = await Repository.StreamGroup.GetStreamGroupByName(m3uFile.DefaultStreamGroupName);
                    if (sg != null)
                    {
                        oldSGId = sg.Id;
                    }
                }
                defaultNameChange = true;
                m3uFile.DefaultStreamGroupName = request.DefaultStreamGroupName;
                ret.Add(new FieldData(() => m3uFile.DefaultStreamGroupName));
            }

            if (request.MaxStreamCount.HasValue)
            {
                m3uFile.MaxStreamCount = request.MaxStreamCount.Value;
                ret.Add(new FieldData(() => m3uFile.MaxStreamCount));

                CacheManager.M3UMaxStreamCounts.AddOrUpdate(m3uFile.Id, m3uFile.MaxStreamCount, (_, _) => m3uFile.MaxStreamCount);
            }

            if (request.SyncChannels.HasValue)
            {
                m3uFile.SyncChannels = request.SyncChannels.Value;
                ret.Add(new FieldData(() => m3uFile.SyncChannels));
            }

            if (request.AutoUpdate.HasValue)
            {
                m3uFile.AutoUpdate = request.AutoUpdate.Value;
                ret.Add(new FieldData(() => m3uFile.AutoUpdate));
            }

            if (request.HoursToUpdate.HasValue)
            {
                m3uFile.HoursToUpdate = request.HoursToUpdate.Value;
                ret.Add(new FieldData(() => m3uFile.HoursToUpdate));
            }

            Repository.M3UFile.UpdateM3UFile(m3uFile);
            _ = await Repository.SaveAsync().ConfigureAwait(false);

            if (request.SyncChannels.HasValue)
            {
                needsUpdate = true;
                // await Sender.Send(new SyncChannelsRequest(m3uFile.Id, m3uFile.DefaultStreamGroupName), cancellationToken);
            }

            if (defaultNameChange && oldSGId != 0)
            {
                await Sender.Send(new MoveSMChannelsToStreamGroupByM3UFileIdRequest(m3uFile, oldSGId), cancellationToken).ConfigureAwait(false);
                await Repository.SaveAsync().ConfigureAwait(false);
                await dataRefreshService.RefreshAllSMChannels().ConfigureAwait(false);
            }

            if (nameChange)
            {
                string sql = $"UPDATE public.\"SMStreams\" SET \"M3UFileName\"='{m3uFile.Name}' WHERE \"M3UFileId\"={m3uFile.Id};";
                await repositoryContext.ExecuteSqlRawAsync(sql, cancellationToken);
                await dataRefreshService.RefreshSMStreams().ConfigureAwait(false);
            }

            if (needsRefresh)
            {
                await Sender.Send(new RefreshM3UFileRequest(m3uFile.Id, true), cancellationToken).ConfigureAwait(false);
            }

            if (needsUpdate)
            {
                ProcessM3UFileRequest processM3UFileRequest = new(m3uFile.Id, true);
                await taskQueue.ProcessM3UFile(processM3UFileRequest, cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            if (ret.Count > 0)
            {
                if (request.SyncChannels == true)
                {
                    await Sender.Send(new SyncChannelsRequest(m3uFile.Id, m3uFile.DefaultStreamGroupName), cancellationToken);
                }
                await dataRefreshService.SetField(ret).ConfigureAwait(false);
                await dataRefreshService.Refresh("GetM3UFileNames").ConfigureAwait(false);
            }
            _ = await Repository.SaveAsync().ConfigureAwait(false);

            //if (request.AutoSetChannelNumbers.HasValue)
            //{
            //    await dataRefreshService.RefreshAllM3U().ConfigureAwait(false);
            //}

            jobManager.SetSuccessful();
            return APIResponse.Success;
        }
        catch (Exception ex)
        {
            jobManager.SetError();
            return APIResponse.ErrorWithMessage(ex, "Failed M3U update");
        }
    }
}
