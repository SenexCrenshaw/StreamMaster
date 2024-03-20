using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMChannelsRepository(ILogger<SMChannelsRepository> intLogger, IRepositoryWrapper repository, IRepositoryContext repositoryContext, IMapper mapper, IIconService iconService)
    : RepositoryBase<SMChannel>(repositoryContext, intLogger), ISMChannelsRepository
{
    public List<SMChannelDto> GetSMChannels()
    {
        return [.. FindAll().Include(a => a.SMStreams).ThenInclude(a => a.SMStream).ProjectTo<SMChannelDto>(mapper.ConfigurationProvider)];
    }

    public IQueryable<SMChannel> GetQuery(bool tracking = false)
    {
        return tracking ? FindAllWithTracking().Include(a => a.SMStreams).ThenInclude(a => a.SMStream) : FindAll().Include(a => a.SMStreams).ThenInclude(a => a.SMStream);
    }

    public PagedResponse<SMChannelDto>? CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<SMChannelDto>(Count());
    }

    public async Task<PagedResponse<SMChannelDto>> GetPagedSMChannels(SMChannelParameters parameters)
    {
        IQueryable<SMChannel> query = GetIQueryableForEntity(parameters).Include(a => a.SMStreams).ThenInclude(a => a.SMStream);
        return await query.GetPagedResponseAsync<SMChannel, SMChannelDto>(parameters.PageNumber, parameters.PageSize, mapper)
                          .ConfigureAwait(false);
    }

    public async Task CreateSMChannel(SMChannel sMChannel)
    {
        Create(sMChannel);
        await SaveChangesAsync();
    }

    public async Task DeleteSMChannel(int smchannelId)
    {
        try
        {

            SMChannel? channel = GetQuery(true).FirstOrDefault(a => a.Id == smchannelId);
            if (channel == null)
            {
                return;
            }
            await repository.SMChannelStreamLink.DeleteSMChannelStreamLinksFromParentId(smchannelId);
            await SaveChangesAsync();
            Delete(channel);
            await SaveChangesAsync();
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SMChannel with id {smchannelId}", smchannelId);
        }
    }

    public async Task<List<int>> DeleteAllSMChannelsFromParameters(SMChannelParameters parameters)
    {
        IQueryable<SMChannel> toDelete = GetIQueryableForEntity(parameters);
        return await DeleteSMChannelsAsync(toDelete).ConfigureAwait(false);
    }

    public SMChannel? GetSMChannel(int smchannelId)
    {
        return GetQuery(true).FirstOrDefault(a => a.Id == smchannelId);
    }

    public async Task<DefaultAPIResponse> CreateSMChannelFromStream(string streamId)
    {
        SMStreamDto? smStream = repository.SMStream.GetSMStream(streamId);
        if (smStream == null)
        {
            return APIResponseFactory.NotFound;
        }

        SMChannel smChannel = new()
        {
            ChannelNumber = smStream.Tvg_chno,
            Group = smStream.Group,
            Name = smStream.Name,
            Logo = smStream.Logo,
            EPGId = smStream.EPGID,
            StationId = smStream.StationId
        };

        await CreateSMChannel(smChannel);

        await repository.SMChannelStreamLink.CreateSMChannelStreamLink(smChannel.Id, smStream.Id);
        return APIResponseFactory.Ok;
    }

    public async Task<DefaultAPIResponse> DeleteSMChannels(List<int> smchannelIds)
    {
        IQueryable<SMChannel> toDelete = GetQuery(true).Where(a => smchannelIds.Contains(a.Id));
        if (!toDelete.Any())
        {
            return APIResponseFactory.NotFound;
        }

        await DeleteSMChannelsAsync(toDelete);

        return APIResponseFactory.Ok;
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
            return APIResponseFactory.NotFound;
        }

        await repository.SMChannelStreamLink.CreateSMChannelStreamLink(SMChannelId, SMStreamId);
        await SaveChangesAsync();

        return APIResponseFactory.Ok;
    }

    public async Task<DefaultAPIResponse> RemoveSMStreamFromSMChannel(int SMChannelId, string SMStreamId)
    {
        IQueryable<SMChannelStreamLink> toDelete = repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == SMChannelId && a.SMStreamId == SMStreamId);
        if (!toDelete.Any())
        {
            return APIResponseFactory.NotFound;
        }
        await repository.SMChannelStreamLink.DeleteSMChannelStreamLinks(toDelete);

        return APIResponseFactory.Ok;
    }

    public async Task<DefaultAPIResponse> SetSMStreamRanks(List<SMChannelRankRequest> request)
    {
        return await repository.SMChannelStreamLink.SetSMStreamRank(request);

    }

    public async Task<string?> SetSMChannelLogo(SMChannelLogoRequest request)
    {
        SMChannel? channel = GetSMChannel(request.SMChannelId);
        if (channel == null)
        {
            return null;
        }

        //if (string.IsNullOrEmpty(request.logo))
        //{
        //    channel.Logo = request.logo;
        //    Update(channel);
        //    await SaveChangesAsync();
        //    return "";
        //}

        //List<IconFileDto> icons = iconService.GetIcons();
        //if (icons.Any(a => a.Source == request.logo))
        //{
        //    channel.Logo = request.logo;
        //    Update(channel);
        //    await SaveChangesAsync();
        //}
        channel.Logo = request.logo;
        Update(channel);
        await SaveChangesAsync();

        return request.logo;
    }
}
