using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.VideoStreams;
using StreamMasterApplication.VideoStreams.Commands;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities;
using StreamMasterDomain.Enums;
using StreamMasterDomain.Extensions;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext
{
    public async Task AddOrUpdateChildToVideoStreamAsync(int parentId, int childId, int rank, CancellationToken cancellationToken)
    {
        var videoStreamLink = await VideoStreamLinks
            .FirstOrDefaultAsync(vsl => vsl.ParentVideoStreamId == parentId && vsl.ChildVideoStreamId == childId, cancellationToken).ConfigureAwait(false);

        if (videoStreamLink == null)
        {
            videoStreamLink = new VideoStreamLink
            {
                ParentVideoStreamId = parentId,
                ChildVideoStreamId = childId,
                Rank = rank
            };

            await VideoStreamLinks.AddAsync(videoStreamLink, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            videoStreamLink.Rank = rank;
        }

        await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteVideoStreamAsync(int videoStreamId, CancellationToken cancellationToken)
    {
        // Get the VideoStream
        var videoStream = await VideoStreams.FindAsync(new object[] { videoStreamId }, cancellationToken).ConfigureAwait(false);
        if (videoStream == null)
        {
            return false;
        }

        // Remove associated VideoStreamLinks where the VideoStream is a parent
        var parentLinks = await VideoStreamLinks
            .Where(vsl => vsl.ParentVideoStreamId == videoStreamId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        VideoStreamLinks.RemoveRange(parentLinks);

        // Remove associated VideoStreamLinks where the VideoStream is a child
        var childLinks = await VideoStreamLinks
            .Where(vsl => vsl.ChildVideoStreamId == videoStreamId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        VideoStreamLinks.RemoveRange(childLinks);

        // Remove the VideoStream
        VideoStreams.Remove(videoStream);

        // Save changes
        try
        {
            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<VideoStream>> GetChildVideoStreamsAsync(int parentId, CancellationToken cancellationToken)
    {
        return await VideoStreamLinks
            .Where(vsl => vsl.ParentVideoStreamId == parentId)
            .Select(vsl => vsl.ChildVideoStream)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<VideoStream> GetVideoStreamWithChildrenAsync(int videoStreamId, CancellationToken cancellationToken)
    {
        return await VideoStreams
            .Include(vs => vs.ChildVideoStreams)
                .ThenInclude(vsl => vsl.ChildVideoStream)
            .SingleOrDefaultAsync(vs => vs.Id == videoStreamId, cancellationToken).ConfigureAwait(false);
    }
}

public partial class AppDbContext : IVideoStreamDB
{
    public DbSet<VideoStreamLink> VideoStreamLinks { get; set; }
    public DbSet<VideoStream> VideoStreams { get; set; }

    public async Task<List<VideoStream>> DeleteVideoStreamsByM3UFiledId(int M3UFileId, CancellationToken cancellationToken)
    {
        var streams = VideoStreams.Where(a => a.M3UFileId == M3UFileId).ToList();

        foreach (var stream in streams)
        {
            await DeleteVideoStreamAsync(stream.Id, cancellationToken).ConfigureAwait(false);
        }

        return streams;
    }

    public async Task<List<VideoStream>> GetAllVideoStreamsWithChildrenAsync()
    {
        return await VideoStreams
            .Include(vs => vs.ChildVideoStreams)
            .ThenInclude(vsl => vsl.ChildVideoStream)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<VideoStream>> GetChildVideoStreamsAsync(int parentId)
    {
        return await VideoStreamLinks
            .Where(vsl => vsl.ParentVideoStreamId == parentId)
            .Select(vsl => vsl.ChildVideoStream)
            .ToListAsync().ConfigureAwait(false);
    }

    public M3UFileIdMaxStream? GetM3UFileIdMaxStreamFromUrl(string Url)
    {
        var videoStream = VideoStreams.FirstOrDefault(a => a.User_Url == Url);

        if (videoStream == null)
        {
            return null;
        }

        if (videoStream.M3UFileId == 0)
        {
            return new M3UFileIdMaxStream { M3UFileId = videoStream.M3UFileId, MaxStreams = 999 };
        }

        var m3uFile = M3UFiles.Single(a => a.Id == videoStream.M3UFileId);

        return new M3UFileIdMaxStream { M3UFileId = videoStream.M3UFileId, MaxStreams = m3uFile.MaxStreamCount };
    }

    public async Task<(VideoStreamHandlers videoStreamHandler, List<ChildVideoStreamDto> childVideoStreamDtos)?> GetStreamsFromVideoStreamById(int videoStreamId, CancellationToken cancellationToken = default)
    {
        var videoStream = await GetVideoStreamWithChildrenAsync(videoStreamId, cancellationToken).ConfigureAwait(false);
        if (videoStream == null)
        {
            return null;
        }

        if (!videoStream.ChildVideoStreams.Any())
        {
            var childVideoStreamDto = _mapper.Map<ChildVideoStreamDto>(videoStream);
            var result = GetM3UFileIdMaxStreamFromUrl(childVideoStreamDto.User_Url);
            if (result == null)
            {
                return null;
            }
            childVideoStreamDto.MaxStreams = result.MaxStreams;
            childVideoStreamDto.M3UFileId = result.M3UFileId;
            return (videoStream.VideoStreamHandler, new List<ChildVideoStreamDto> { childVideoStreamDto });
        }

        var childVideoStreams = videoStream.ChildVideoStreams.OrderBy(a => a.Rank).Select(a => a.ChildVideoStream).ToList();
        var childVideoStreamDtos = _mapper.Map<List<ChildVideoStreamDto>>(childVideoStreams);

        foreach (var childVideoStreamDto in childVideoStreamDtos)
        {
            var result = GetM3UFileIdMaxStreamFromUrl(childVideoStreamDto.User_Url);
            if (result == null)
            {
                return null;
            }
            childVideoStreamDto.M3UFileId = result.M3UFileId;
            childVideoStreamDto.MaxStreams = result.MaxStreams;
        }

        return (videoStream.VideoStreamHandler, childVideoStreamDtos);
    }

    public async Task<List<StreamGroupDto>> GetStreamGroupsByVideoStreamIdsAsync(List<int> videoStreamIds,string url, CancellationToken cancellationToken)
    {
        var sgs = await GetStreamGroupDtos(url,cancellationToken);

        var matchingStreamGroups = sgs
            .Where(sg => sg.ChildVideoStreams.Any(sgvs => videoStreamIds.Contains(sgvs.Id)))
            .ToList();

        return matchingStreamGroups;
    }


    public async Task<VideoStreamDto> GetVideoStreamDtoWithChildrenAsync(int videoStreamId, CancellationToken cancellationToken)
    {
        var stream = await GetVideoStreamWithChildrenAsync(videoStreamId, cancellationToken).ConfigureAwait(false);

        VideoStreamDto videoStreamDto = _mapper.Map<VideoStreamDto>(stream);

        return videoStreamDto;
    }

    public async Task RemoveNonExistingVideoStreamLinksAsync(List<ChildVideoStreamDto> existingVideoStreamLinks, CancellationToken cancellationToken)
    {
        var existingLinkIds = existingVideoStreamLinks.Select(vsl => vsl.Id).ToList();

        var linksToRemove = await VideoStreamLinks
            .Where(vsl => !existingLinkIds.Contains(vsl.ChildVideoStreamId))
            .ToListAsync(cancellationToken);

        VideoStreamLinks.RemoveRange(linksToRemove);

        await SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SynchronizeChildRelationships(VideoStream videoStream, List<ChildVideoStreamDto> childVideoStreams, CancellationToken cancellationToken)
    {
        bool isChanged = false;
        try
        {
            foreach (var ch in childVideoStreams)
            {
                await AddOrUpdateChildToVideoStreamAsync(videoStream.Id, ch.Id, ch.Rank, cancellationToken).ConfigureAwait(false);
            }

            await RemoveNonExistingVideoStreamLinksAsync(childVideoStreams.ToList(), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Handle the exception or log the error
            throw;
        }
        return isChanged;
    }

    public async Task<VideoStreamDto?> UpdateVideoStreamAsync(UpdateVideoStreamRequest request, CancellationToken cancellationToken)
    {
        VideoStream? videoStream = await GetVideoStreamWithChildrenAsync(request.Id, cancellationToken).ConfigureAwait(false);

        if (videoStream == null)
        {
            return null;
        }
        Setting setting = FileUtil.GetSetting();

        List<IconFileDto> icons = await GetIcons(cancellationToken).ConfigureAwait(false);

        bool isChanged = videoStream.UpdateVideoStream(request);

        var newLogo = videoStream.User_Tvg_logo;

        if (request.Tvg_logo != null && videoStream.User_Tvg_logo != request.Tvg_logo)
        {
            isChanged = true;

            IconFileDto? logo = icons.FirstOrDefault(a => a.OriginalSource == request.Tvg_logo);

            if (logo != null)
            {
                videoStream.User_Tvg_logo = logo.OriginalSource;
                newLogo = logo.Source;
            }
            else
            {
                videoStream.User_Tvg_logo = request.Tvg_logo;
            }
        }

        _ = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        if (request.ChildVideoStreams != null)
        {
            isChanged = isChanged || await SynchronizeChildRelationships(videoStream, request.ChildVideoStreams, cancellationToken).ConfigureAwait(false);
        }

        _ = await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        VideoStreamDto ret = _mapper.Map<VideoStreamDto>(videoStream);

        IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == videoStream.User_Tvg_logo);
        string Logo = icon != null ? icon.Source : "/" + setting.DefaultIcon;
        ret.User_Tvg_logo = Logo;

        return ret;
    }
}
