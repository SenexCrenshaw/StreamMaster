using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;

using System.Linq.Expressions;

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
        return await query.GetPagedResponseAsync<SMChannel, SMChannelDto>(parameters.PageNumber, parameters.PageSize, mapper)
                          .ConfigureAwait(false);
    }

    public async Task CreateSMChannel(SMChannel sMChannel)
    {
        Create(sMChannel);
        await SaveChangesAsync();
    }

    public async Task<DefaultAPIResponse> DeleteSMChannel(int smchannelId)
    {
        try
        {
            SMChannel? channel = await FirstOrDefaultAsync(a => a.Id == smchannelId);
            if (channel == null)
            {
                return DefaultAPIResponse.ErrorWithMessage("SMChannel not found");
            }

            Delete(channel);
            await SaveChangesAsync();
            return DefaultAPIResponse.OkWithMessage(channel.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SMChannel with id {smchannelId}", smchannelId);
        }
        return DefaultAPIResponse.ErrorWithMessage($"Error deleting SMChannel with id {smchannelId}");
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

    public async Task<DefaultAPIResponse> CreateSMChannelFromStream(string streamId)
    {
        SMStreamDto? smStream = repository.SMStream.GetSMStream(streamId);
        if (smStream == null)
        {
            return DefaultAPIResponse.NotFound;
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
        return DefaultAPIResponse.Success;
    }

    public async Task<DefaultAPIResponse> DeleteSMChannels(List<int> smchannelIds)
    {
        IQueryable<SMChannel> toDelete = GetQuery(true).Where(a => smchannelIds.Contains(a.Id));
        if (!toDelete.Any())
        {
            return DefaultAPIResponse.NotFound;
        }

        await DeleteSMChannelsAsync(toDelete);

        return DefaultAPIResponse.Success;
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
            List<int> ret = [.. channels.Select(a => a.Id)];
            IQueryable<SMChannelStreamLink> linksToDelete = repository.SMChannelStreamLink.GetQuery(true).Where(a => ret.Contains(a.SMChannelId));
            await repository.SMChannelStreamLink.DeleteSMChannelStreamLinks(linksToDelete);
            await SaveChangesAsync();
            await BulkDeleteAsync(channels).ConfigureAwait(false);
            await SaveChangesAsync();
            return ret;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SMChannels");
        }
        return [];
    }

    public async Task<DefaultAPIResponse> AddSMStreamToSMChannel(int SMChannelId, string SMStreamId)
    {
        if (GetSMChannel(SMChannelId) == null || repository.SMStream.GetSMStream(SMStreamId) == null)
        {
            return DefaultAPIResponse.NotFound;
        }

        await repository.SMChannelStreamLink.CreateSMChannelStreamLink(SMChannelId, SMStreamId);
        await SaveChangesAsync();

        return DefaultAPIResponse.Success;
    }

    public async Task<DefaultAPIResponse> RemoveSMStreamFromSMChannel(int SMChannelId, string SMStreamId)
    {
        IQueryable<SMChannelStreamLink> toDelete = repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == SMChannelId && a.SMStreamId == SMStreamId);
        if (!toDelete.Any())
        {
            return DefaultAPIResponse.NotFound;
        }
        await repository.SMChannelStreamLink.DeleteSMChannelStreamLinks(toDelete);

        return DefaultAPIResponse.Success;
    }

    public async Task<DefaultAPIResponse> SetSMStreamRanks(List<SMChannelRankRequest> request)
    {
        return await repository.SMChannelStreamLink.SetSMStreamRank(request);

    }

    public async Task<DefaultAPIResponse> SetSMChannelLogo(int SMChannelId, string logo)
    {
        SMChannel? channel = GetSMChannel(SMChannelId);
        if (channel == null)
        {
            return DefaultAPIResponse.Success;
        }

        channel.Logo = logo;
        Update(channel);
        await SaveChangesAsync();

        return DefaultAPIResponse.Success;
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

    public async Task<DefaultAPIResponse> SetSMChannelChannelNumber(int sMChannelId, int channelNumber)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return DefaultAPIResponse.NotFound;
        }

        channel.ChannelNumber = channelNumber;
        Update(channel);
        await SaveChangesAsync();

        return DefaultAPIResponse.Success;
    }

    public async Task<DefaultAPIResponse> SetSMChannelName(int sMChannelId, string name)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return DefaultAPIResponse.NotFound;
        }

        channel.Name = name;
        Update(channel);
        await SaveChangesAsync();

        return DefaultAPIResponse.Success;
    }
}
