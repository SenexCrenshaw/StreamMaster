using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.VideoStreams;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities;

using System.Threading;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : IVideoStreamDB
{
    public DbSet<VideoStreamRelationship> VideoStreamRelationships { get; set; }
    public DbSet<VideoStream> VideoStreams { get; set; }

    public async Task<bool> DeleteVideoStream(int VideoStreamId)
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

        var sgStreams = StreamGroups.Include(a => a.VideoStreams).Where(a => a.VideoStreams.Any(a => a.Id == VideoStreamId)).ToList();

        VideoStreamRelationships.RemoveRange(relationsShips);
        StreamGroups.RemoveRange(sgStreams);
        VideoStreams.Remove(VideoStream);

        _ = await SaveChangesAsync().ConfigureAwait(false);
        return true;
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