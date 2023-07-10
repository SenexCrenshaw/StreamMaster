using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.VideoStreams;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities;

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

    public bool SynchronizeChildRelationships(VideoStream videoStream, List<ChildVideoStreamDto> childVideoStreams)
    {
        bool isChanged = false;
        try
        {
            foreach (var item in videoStream.ParentRelationships)
            {
                if (!childVideoStreams.Any(a => a.Id == item.ChildVideoStreamId))
                {
                    VideoStreamRelationships.Remove(item);
                    isChanged = true;
                }
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
