using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;
namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMStreamRepository(ILogger<SMStreamRepository> intLogger, IRepositoryWrapper repository, IRepositoryContext repositoryContext, IOptionsMonitor<Setting> intSettings, IMapper mapper)
    : RepositoryBase<SMStream>(repositoryContext, intLogger, intSettings),
    ISMStreamRepository
{
    public async Task<(VideoStreamHandlers videoStreamHandler, List<SMStreamDto> childSMStreamDtos)?> GetStreamsFromSMStreamById(string SMStreamId, CancellationToken cancellationToken = default)
    {
        var videoStream = await GetQuery(a => a.Id == SMStreamId)
            .Include(a => a.ChildVideoStreams)
            .ThenInclude(a => a.ChildVideoStream)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        if (videoStream == null)
        {
            return null;
        }

        VideoStreamDto videoStreamDto = mapper.Map<VideoStreamDto>(videoStream);
        if (videoStream.ChildVideoStreams is null || !videoStream.ChildVideoStreams.Any())
        {

            M3UFileIdMaxStream? result = await sender.Send(new GetM3UFileIdMaxStreamFromUrlQuery(videoStreamDto.User_Url), cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }
            videoStreamDto.MaxStreams = result.MaxStreams;
            videoStreamDto.M3UFileId = result.M3UFileId;
            return (videoStream.VideoStreamHandler, new List<VideoStreamDto> { videoStreamDto });
        }


        List<VideoStream> childVideoStreams =
        [
            .. RepositoryContext.VideoStreamLinks
                    .Include(a => a.ChildVideoStream)
                    .Where(a => a.ParentVideoStreamId == videoStream.Id)
                    .Select(a => a.ChildVideoStream),
        ];

        List<VideoStreamDto> childVideoStreamDtos = mapper.Map<List<VideoStreamDto>>(childVideoStreams);
        if (!string.IsNullOrEmpty(videoStreamDto.User_Url))
        {
            childVideoStreamDtos.Insert(0, videoStreamDto);
        }

        Dictionary<string, M3UFileIdMaxStream?> m3uCache = [];

        foreach (VideoStreamDto childVideoStreamDto in childVideoStreamDtos)
        {
            if (m3uCache.TryGetValue(childVideoStreamDto.User_Url, out M3UFileIdMaxStream? cachedResult) && cachedResult != null)
            {
                childVideoStreamDto.M3UFileId = cachedResult.M3UFileId;
                childVideoStreamDto.MaxStreams = cachedResult.MaxStreams;
                continue;
            }

            M3UFileIdMaxStream? result = await sender.Send(new GetM3UFileIdMaxStreamFromUrlQuery(childVideoStreamDto.User_Url), cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return null;
            }
            VideoStreamLink? link = videoStream.ChildVideoStreams.FirstOrDefault(a => a.ChildVideoStreamId == childVideoStreamDto.Id);
            if (link is not null)
            {
                childVideoStreamDto.Rank = link.Rank;
            }
            childVideoStreamDto.M3UFileId = result.M3UFileId;
            childVideoStreamDto.MaxStreams = result.MaxStreams;
            m3uCache[childVideoStreamDto.User_Url] = result;
        }

        return (videoStream.VideoStreamHandler, childVideoStreamDtos);
    }
    public List<SMStreamDto> GetSMStreams()
    {
        return [.. GetQuery().ProjectTo<SMStreamDto>(mapper.ConfigurationProvider)];
    }

    public PagedResponse<SMStreamDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<SMStreamDto>(Count());
    }

    public async Task<PagedResponse<SMStreamDto>> GetPagedSMStreams(QueryStringParameters parameters, CancellationToken cancellationToken)
    {
        IQueryable<SMStream> query = GetQuery(parameters);


        return await query.GetPagedResponseAsync<SMStream, SMStreamDto>(parameters.PageNumber, parameters.PageSize, mapper)
                          .ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> DeleteAllSMStreamsFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken)
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

        }

        return videoStreamIds;
    }

    public async Task<SMStreamDto?> DeleteSMStreamById(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        SMStream? stream = await FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken).ConfigureAwait(false);
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

        SMStream? stream = await FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (stream == null)
        {
            return null;
        }
        stream.IsHidden = !stream.IsHidden;
        Update(stream);
        await SaveChangesAsync();
        return mapper.Map<SMStreamDto>(stream);
    }

    public SMStreamDto? GetSMStream(string streamId)
    {
        SMStream? channel = FirstOrDefault(a => a.Id == streamId);
        return channel == null ? null : mapper.Map<SMStreamDto>(channel);
    }

    public async Task DeleteSMStreamsByM3UFiledId(int id, CancellationToken cancellationToken)
    {
        IQueryable<SMStream> query = GetQuery(a => a.M3UFileId == id);

        List<string> videoStreamIds = [.. query.Select(vs => vs.Id)];

        if (!query.Any())
        {
            return;
        }

        IQueryable<SMChannelStreamLink> childLinks = repository.SMChannelStreamLink.GetQuery().Where(vsl => videoStreamIds.Contains(vsl.SMStreamId));
        await childLinks.ExecuteDeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        await query.ExecuteDeleteAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return;
    }
    public async Task<List<FieldData>> ToggleSMStreamVisibleByParameters(QueryStringParameters parameters, CancellationToken cancellationToken)
    {
        IQueryable<SMStream> query = GetQuery(parameters);
        return await ToggleSMStreamsVisibleById([.. query.Select(a => a.Id)], cancellationToken);
    }
    public async Task<List<FieldData>> ToggleSMStreamsVisibleById(List<string> ids, CancellationToken cancellationToken)
    {
        List<FieldData> ret = [];
        IQueryable<SMStream> streams = GetQuery(true).Where(a => ids.Contains(a.Id));
        foreach (SMStream? stream in streams)
        {
            stream.IsHidden = !stream.IsHidden;
            ret.Add(new FieldData(() => stream.IsHidden));
        }
        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return ret;
    }

    public async Task<List<FieldData>> SetSMStreamsVisibleById(List<string> ids, bool isHidden, CancellationToken cancellationToken)
    {
        List<FieldData> ret = [];
        IQueryable<SMStream> streams = GetQuery(true).Where(a => ids.Contains(a.Id));
        foreach (SMStream stream in streams)
        {
            stream.IsHidden = isHidden;
            ret.Add(new FieldData(() => stream.IsHidden));
        }
        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return ret;
    }
}
