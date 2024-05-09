using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Exceptions;
using StreamMaster.Domain.Filtering;

using System.Linq.Expressions;
using System.Text.Json;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMChannelsRepository(ILogger<SMChannelsRepository> intLogger, IRepositoryWrapper repository, IRepositoryContext repositoryContext, IMapper mapper, IOptionsMonitor<Setting> intSettings, IIconService iconService)
    : RepositoryBase<SMChannel>(repositoryContext, intLogger, intSettings), ISMChannelsRepository
{

    public List<SMChannelDto> GetSMChannels()
    {
        return [.. GetQuery().Include(a => a.SMStreams).ThenInclude(a => a.SMStream).ProjectTo<SMChannelDto>(mapper.ConfigurationProvider)];
    }

    public PagedResponse<SMChannelDto>? CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<SMChannelDto>(Count());
    }

    public async Task<PagedResponse<SMChannelDto>> GetPagedSMChannels(QueryStringParameters parameters)
    {
        IQueryable<SMChannel> query = GetQuery(parameters).Include(a => a.SMStreams).ThenInclude(a => a.SMStream);

        if (!string.IsNullOrEmpty(parameters.JSONFiltersString))
        {
            List<DataTableFilterMetaData>? filters = JsonSerializer.Deserialize<List<DataTableFilterMetaData>>(parameters.JSONFiltersString);
            if (filters != null && filters.Any(a => a.MatchMode == "inSG"))
            {
                DataTableFilterMetaData? inSGFilter = filters.FirstOrDefault(a => a.MatchMode == "inSG");
                if (inSGFilter != null && inSGFilter.Value != null)
                {
                    try
                    {
                        var streamGroupIdString = inSGFilter.Value.ToString();
                        if (!string.IsNullOrWhiteSpace(streamGroupIdString))
                        {
                            var streamGroupId = Convert.ToInt32(streamGroupIdString);
                            var linkIds = repository.StreamGroupSMChannelLink.GetQuery().Where(a => a.StreamGroupId == streamGroupId).Select(a => a.SMChannelId).ToList();
                            query = query.Where(a => linkIds.Contains(a.Id));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Object value is outside the range of an Int32. Object: {Object}", inSGFilter.Value);
                    }

                }
            }

            if (filters != null && filters.Any(a => a.MatchMode == "notInSG"))
            {
                DataTableFilterMetaData? notInSGFilter = filters.FirstOrDefault(a => a.MatchMode == "notInSG");
                if (notInSGFilter != null && notInSGFilter.Value != null)
                {
                    try
                    {
                        var streamGroupIdString = notInSGFilter.Value.ToString();
                        if (!string.IsNullOrWhiteSpace(streamGroupIdString))
                        {
                            var streamGroupId = Convert.ToInt32(streamGroupIdString);
                            var linkIds = repository.StreamGroupSMChannelLink.GetQuery().Where(a => a.StreamGroupId == streamGroupId).Select(a => a.SMChannelId).ToList();
                            query = query.Where(a => !linkIds.Contains(a.Id));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Object value is outside the range of an Int32. Object: {Object}", notInSGFilter.Value);
                    }

                }

            }
        }

        return await query.GetPagedResponseAsync<SMChannel, SMChannelDto>(parameters.PageNumber, parameters.PageSize, mapper)
                              .ConfigureAwait(false);
    }

    public async Task CreateSMChannel(SMChannel sMChannel)
    {
        Create(sMChannel);
        await SaveChangesAsync();
    }

    public async Task<APIResponse> DeleteSMChannel(int smchannelId)
    {
        try
        {
            SMChannel? channel = await FirstOrDefaultAsync(a => a.Id == smchannelId);
            if (channel == null)
            {
                return APIResponse.ErrorWithMessage("SMChannel not found");
            }

            Delete(channel);
            await SaveChangesAsync();
            return APIResponse.OkWithMessage(channel.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SMChannel with id {smchannelId}", smchannelId);
            return APIResponse.ErrorWithMessage(ex, $"Error deleting SMChannel with id {smchannelId}");
        }

    }

    public async Task<List<int>> DeleteSMChannelsFromParameters(QueryStringParameters parameters)
    {
        IQueryable<SMChannel> toDelete = GetQuery(parameters);
        return await DeleteSMChannelsAsync(toDelete).ConfigureAwait(false);
    }

    public SMChannel? GetSMChannel(int smchannelId)
    {
        return FirstOrDefault(a => a.Id == smchannelId, tracking: false);
    }

    public async Task<APIResponse> CreateSMChannelFromStream(string streamId)
    {
        SMStreamDto? smStream = repository.SMStream.GetSMStream(streamId);
        if (smStream == null)
        {
            throw new APIException($"Stream with Id {streamId} is not found");
        }

        SMChannel smChannel = new()
        {
            ChannelNumber = smStream.ChannelNumber,
            Group = smStream.Group,
            Name = smStream.Name,
            Logo = smStream.Logo,
            EPGId = smStream.EPGID,
            StationId = smStream.StationId
        };

        await CreateSMChannel(smChannel);

        await repository.SMChannelStreamLink.CreateSMChannelStreamLink(smChannel.Id, smStream.Id);
        return APIResponse.Success;
    }

    public async Task<APIResponse> DeleteSMChannels(List<int> smchannelIds)
    {
        IQueryable<SMChannel> toDelete = GetQuery(true).Where(a => smchannelIds.Contains(a.Id));
        if (!toDelete.Any())
        {
            return APIResponse.NotFound;
        }

        await DeleteSMChannelsAsync(toDelete);

        return APIResponse.Success;
    }


    [LogExecutionTimeAspect]
    private async Task<List<int>> DeleteSMChannelsAsync(IQueryable<SMChannel> channels)
    {
        if (!channels.Any())
        {
            return [];
        }
        try
        {
            var a = channels.ToList();
            List<int> ret = [.. a.Select(a => a.Id)];
            IQueryable<SMChannelStreamLink> linksToDelete = repository.SMChannelStreamLink.GetQuery(true).Where(a => ret.Contains(a.SMChannelId));
            await repository.SMChannelStreamLink.DeleteSMChannelStreamLinks(linksToDelete);
            await SaveChangesAsync();
            BulkDelete(a);
            await SaveChangesAsync();
            return ret;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SMChannels");
        }
        return [];
    }

    public async Task<APIResponse> AddSMStreamToSMChannel(int SMChannelId, string SMStreamId)
    {
        if (GetSMChannel(SMChannelId) == null || repository.SMStream.GetSMStream(SMStreamId) == null)
        {
            throw new APIException($"Channel with Id {SMChannelId} or stream with Id {SMStreamId} not found");
        }

        await repository.SMChannelStreamLink.CreateSMChannelStreamLink(SMChannelId, SMStreamId);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public async Task<APIResponse> RemoveSMStreamFromSMChannel(int SMChannelId, string SMStreamId)
    {
        IQueryable<SMChannelStreamLink> toDelete = repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == SMChannelId && a.SMStreamId == SMStreamId);
        if (!toDelete.Any())
        {
            throw new APIException($"Channel with id {SMChannelId} does not contain stream with Id {SMStreamId}");
        }
        await repository.SMChannelStreamLink.DeleteSMChannelStreamLinks(toDelete);

        return APIResponse.Success;
    }

    public async Task<APIResponse> SetSMStreamRanks(List<SMChannelRankRequest> request)
    {
        return await repository.SMChannelStreamLink.SetSMStreamRank(request);

    }

    public async Task<APIResponse> SetSMChannelLogo(int SMChannelId, string logo)
    {
        SMChannel? channel = GetSMChannel(SMChannelId);
        if (channel == null)
        {
            return APIResponse.ErrorWithMessage($"Channel {SMChannelId} doesnt exist");
        }

        channel.Logo = logo;
        Update(channel);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public override IQueryable<SMChannel> GetQuery(bool tracking = false)
    {
        return tracking
            ? base.GetQuery(tracking).Include(a => a.SMStreams).ThenInclude(a => a.SMStream)
            : base.GetQuery(tracking).Include(a => a.SMStreams).ThenInclude(a => a.SMStream).AsNoTracking();
    }

    public override IQueryable<SMChannel> GetQuery(Expression<Func<SMChannel, bool>> expression, bool tracking = false)
    {
        return base.GetQuery(expression, tracking).Include(a => a.SMStreams).ThenInclude(a => a.SMStream);
    }

    public async Task<APIResponse> SetSMChannelChannelNumber(int sMChannelId, int channelNumber)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        channel.ChannelNumber = channelNumber;
        Update(channel);
        await SaveChangesAsync();

        return APIResponse.Success;
    }


    public async Task<APIResponse> SetSMChannelName(int sMChannelId, string name)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        channel.Name = name;
        Update(channel);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public async Task<APIResponse> SetSMChannelEPGID(int sMChannelId, string EPGId)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        channel.EPGId = EPGId;
        Update(channel);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public async Task<APIResponse> SetSMChannelGroup(int sMChannelId, string group)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        channel.Group = group;
        Update(channel);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public async Task<APIResponse> CopySMChannel(int sMChannelId, string newName)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        var newChannel = channel.DeepCopy();
        newChannel.Id = 0;

        newChannel.Name = newName;
        await CreateSMChannel(newChannel);
        await SaveChangesAsync();
        var links = repository.SMChannelStreamLink.GetQuery().Where(a => a.SMChannelId == sMChannelId).ToList();

        foreach (var link in links)
        {
            var newLink = new SMChannelStreamLink
            {
                SMChannelId = newChannel.Id,
                SMStreamId = link.SMStreamId,
                Rank = link.Rank,
            };

            repository.SMChannelStreamLink.Create(newLink);
        }
        await SaveChangesAsync();

        return APIResponse.Success;

    }

    public async Task<APIResponse> CreateSMChannelFromStreams(List<string> streamIds)
    {
        try
        {
            foreach (var streamId in streamIds)
            {
                var resp = await CreateSMChannelFromStream(streamId);
                if (resp.IsError)
                {
                    return resp;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating SMChannel from streams");
            return APIResponse.ErrorWithMessage(ex, "Error creating SMChannel from streams");
        }

        return APIResponse.Success;
    }

    public Task<APIResponse> CreateSMChannelFromStreamParameters(QueryStringParameters parameters)
    {
        IQueryable<SMStream> toCreate = repository.SMStream.GetQuery(parameters);
        return CreateSMChannelFromStreams(toCreate.Select(a => a.Id).ToList());
    }

    public async Task<List<FieldData>> ToggleSMChannelsVisibleById(List<int> ids, CancellationToken cancellationToken)
    {
        List<FieldData> ret = [];
        var channels = GetQuery(true).Where(a => ids.Contains(a.Id)).ToList();

        foreach (var channel in channels)
        {
            channel.IsHidden = !channel.IsHidden;
            ret.Add(new FieldData(() => channel.IsHidden));
        }
        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return ret;
    }

    public async Task<SMChannelDto?> ToggleSMChannelVisibleById(int id, CancellationToken cancellationToken)
    {
        if (id == 0)
        {
            throw new ArgumentNullException(nameof(id));
        }

        var channel = await FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (channel == null)
        {
            return null;
        }
        channel.IsHidden = !channel.IsHidden;
        Update(channel);
        await SaveChangesAsync();
        return mapper.Map<SMChannelDto>(channel);
    }

    public async Task<List<FieldData>> ToggleSMChannelVisibleByParameters(QueryStringParameters parameters, CancellationToken cancellationToken)
    {
        IQueryable<SMChannel> query = GetQuery(parameters);
        return await ToggleSMChannelsVisibleById([.. query.Select(a => a.Id)], cancellationToken);
    }
}
