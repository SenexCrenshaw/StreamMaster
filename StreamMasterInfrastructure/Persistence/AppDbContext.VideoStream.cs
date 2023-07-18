using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.VideoStreams;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities;
using StreamMasterDomain.Enums;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : IVideoStreamDB
{
    public DbSet<VideoStreamRelationship> VideoStreamRelationships { get; set; }

    public DbSet<VideoStream> VideoStreams { get; set; }

    public async Task<bool> DeleteVideoStream(int VideoStreamId, bool save = true)
    {
        var VideoStream = await VideoStreams.Include(a => a.ChildRelationships).FirstOrDefaultAsync(a => a.Id == VideoStreamId).ConfigureAwait(false);
        if (VideoStream == null)
        {
            return false;
        }

        var relationsShips = VideoStreamRelationships
            .Include(a => a.ChildVideoStream)
        .Where(a =>
            a.ParentVideoStreamId == VideoStreamId ||
            a.ChildVideoStreamId == VideoStreamId
        )
        .ToList();

        VideoStreamRelationships.RemoveRange(relationsShips);
        foreach (var sg in StreamGroups)
        {
            sg.VideoStreams.RemoveAll(a => a.Equals(VideoStreamId));
        }

        VideoStreams.Remove(VideoStream);

        if (save)
            _ = await SaveChangesAsync().ConfigureAwait(false);

        return true;
    }

    public async Task<List<VideoStream>> DeleteVideoStreamsByM3UFiledId(int M3UFileId, bool save = true)
    {
        var streams = VideoStreams.Where(a => a.M3UFileId == M3UFileId).ToList();

        foreach (var stream in streams)
        {
            await DeleteVideoStream(stream.Id, false);
        }

        if (save)
            await SaveChangesAsync().ConfigureAwait(false);

        return streams;
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
        var videoStream = await GetVideoStream(videoStreamId, cancellationToken);
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

        var childVideoStreamDtos = videoStream.ChildVideoStreams.OrderBy(a => a.Rank).ToList();
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

    public async Task<VideoStreamDto?> GetVideoStream(int videoStreamId, CancellationToken cancellationToken = default)
    {
        var videoStream = VideoStreams.FirstOrDefault(a => a.Id == videoStreamId);
        if (videoStream == null)
        {
            return null;
        }

        List<IconFileDto> icons = await GetIcons(cancellationToken).ConfigureAwait(false);

        var videoStreams =
            VideoStreamRelationships.
            Include(c => c.ChildVideoStream).
            Where(a => a.ParentVideoStreamId == videoStream.Id).
            Select(a => new
            {
                ChildVideoStream = a.ChildVideoStream,
                Rank = a.Rank
            }).ToList();

        VideoStreamDto videoStreamDto = _mapper.Map<VideoStreamDto>(videoStream);

        if (setting.CacheIcons && !string.IsNullOrEmpty(videoStreamDto.User_Tvg_logo))
        {
            IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == videoStreamDto.User_Tvg_logo || a.Name == videoStreamDto.User_Tvg_logo);
            string Logo = icon != null ? icon.Source : "/" + setting.DefaultIcon;

            videoStreamDto.User_Tvg_logo = Logo;
        }

        var childVideoStreams = new List<ChildVideoStreamDto>();

        foreach (var child in videoStreams)
        {
            if (!string.IsNullOrEmpty(child.ChildVideoStream.User_Tvg_logo))
            {
                if (setting.CacheIcons)
                {
                    IconFileDto? icon = icons.SingleOrDefault(a => a.OriginalSource == child.ChildVideoStream.User_Tvg_logo);
                    string Logo = icon != null ? icon.Source : "/" + setting.DefaultIcon;
                    child.ChildVideoStream.User_Tvg_logo = Logo;
                }
                var cto = _mapper.Map<ChildVideoStreamDto>(child.ChildVideoStream);
                cto.Rank = child.Rank;
                childVideoStreams.Add(cto);
            }
        }

        videoStreamDto.ChildVideoStreams = childVideoStreams;

        return videoStreamDto;
    }

    public bool SynchronizeChildRelationships(VideoStream videoStream, List<ChildVideoStreamDto> childVideoStreams)
    {
        bool isChanged = false;
        try
        {
            if (videoStream.ParentRelationships is not null)
            {
                foreach (var item in videoStream.ParentRelationships)
                {
                    if (!childVideoStreams.Any(a => a.Id == item.ChildVideoStreamId))
                    {
                        VideoStreamRelationships.Remove(item);
                        isChanged = true;
                    }
                }
            }
            else
            {
                videoStream.ParentRelationships = new();
            }

            foreach (var ch in childVideoStreams)
            {
                var test = VideoStreamRelationships.FirstOrDefault(a => a.ParentVideoStreamId == videoStream.Id && a.ChildVideoStreamId == ch.Id);
                if (test != null)
                {
                    if (test.Rank != ch.Rank)
                    {
                        test.Rank = ch.Rank;
                        isChanged = true;
                    }
                }
                else
                {
                    VideoStreamRelationships.Add(new VideoStreamRelationship
                    {
                        ParentVideoStreamId = videoStream.Id,
                        ChildVideoStreamId = ch.Id,
                        Rank = ch.Rank
                    });
                    isChanged = true;
                }
            }
        }
        catch (Exception ex)
        {
            // Handle the exception or log the error
            throw;
        }
        return isChanged;
    }
}
