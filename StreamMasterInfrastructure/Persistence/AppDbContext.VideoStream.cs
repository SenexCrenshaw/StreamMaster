using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.VideoStreams;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : IVideoStreamDB
{
    public DbSet<VideoStreamRelationship> VideoStreamRelationships { get; set; }
    public DbSet<VideoStream> VideoStreams { get; set; }

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
