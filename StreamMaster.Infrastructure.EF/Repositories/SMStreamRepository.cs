using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;
namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMStreamRepository(ILogger<SMStreamRepository> intLogger, IRepositoryContext repositoryContext, IMapper mapper) : RepositoryBase<SMStream>(repositoryContext, intLogger),
    ISMStreamRepository
{
    public List<SMStreamDto> GetSMStreams()
    {
        return [.. GetQuery().ProjectTo<SMStreamDto>(mapper.ConfigurationProvider)];
    }

    public PagedResponse<SMStreamDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<SMStreamDto>(Count());
    }

    public async Task<PagedResponse<SMStreamDto>> GetPagedSMStreams(SMStreamParameters parameters, CancellationToken cancellationToken)
    {
        IQueryable<SMStream> query = GetQuery(parameters);
        return await query.GetPagedResponseAsync<SMStream, SMStreamDto>(parameters.PageNumber, parameters.PageSize, mapper)
                          .ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> DeleteAllSMStreamsFromParameters(SMStreamParameters parameters, CancellationToken cancellationToken)
    {
        IQueryable<SMStream> toDelete = GetQuery(parameters).Where(a => a.IsUserCreated);
        return await DeleteStreamsAsync(toDelete, cancellationToken).ConfigureAwait(false);
    }

    [LogExecutionTimeAspect]
    public async Task<List<string>> DeleteStreamsAsync(IQueryable<SMStream> videoStreams, CancellationToken cancellationToken)
    {

        if (!videoStreams.Any())
        {
            return [];
        }

        // Get the VideoStreams
        List<string> videoStreamIds = [.. videoStreams.Select(vs => vs.Id)];
        List<string> cgNames = [.. videoStreams.Select(vs => vs.Group)];

        int deletedCount = 0;

        // Remove the VideoStreams
        int count = 0;
        int chunkSize = 500;
        int totalCount = videoStreams.Count();
        logger.LogInformation($"Deleting {totalCount} video streams");
        while (count < totalCount)
        {
            // Calculate the size of the next chunk
            int nextChunkSize = Math.Min(chunkSize, totalCount - count);

            int deletedRecords = videoStreams.Take(nextChunkSize).ExecuteDelete();

            count += nextChunkSize;
            logger.LogInformation($"Deleted {count} of {totalCount} video streams");
        }

        deletedCount += videoStreams.Count();

        // Save changes
        try
        {
            _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
            // You can decide how to handle exceptions here, for example by
            // logging them. In this case, we're simply swallowing the exception.
        }

        //foreach (string cgName in cgNames)
        //{
        //    ChannelGroup? cg = await RepositoryContext.ChannelGroups.FirstOrDefaultAsync(a => a.Name == cgName, cancellationToken: cancellationToken).ConfigureAwait(false);
        //    await sender.Send(new SyncStreamGroupChannelGroupByChannelIdRequest(cg.Id), cancellationToken).ConfigureAwait(false);
        //}


        return videoStreamIds;
    }

    public async Task<SMStreamDto?> DeleteSMStreamById(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        SMStream? stream = await GetQuery(a => a.Id == id).FirstOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (stream == null)
        {
            return null;
        }

        Delete(stream);
        logger.LogInformation($"Stream with Name {stream.Name} was deleted.");
        return mapper.Map<SMStreamDto>(stream);
    }

    public async Task<SMStreamDto?> ToggleSMStreamVisibleById(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        SMStream? stream = await FindByConditionTracked(a => a.Id == id).FirstOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (stream == null)
        {
            return null;
        }
        stream.IsHidden = !stream.IsHidden;
        await SaveChangesAsync();
        return mapper.Map<SMStreamDto>(stream);
    }

    public SMStreamDto? GetSMStream(string streamId)
    {
        SMStream? channel = GetQuery().FirstOrDefault(a => a.Id == streamId);
        return channel == null ? null : mapper.Map<SMStreamDto>(channel);
    }
}
