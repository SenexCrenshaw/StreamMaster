using Microsoft.EntityFrameworkCore;

using StreamMasterDomain.Entities;

namespace StreamMasterApplication.Common.Extensions;

public static class VideoStreamExtensions
{
    public static async Task<VideoStream> GetWithChildrenAndParentsAsync(this DbSet<VideoStream> videoStreams, int videoStreamId)
    {
        return await videoStreams
            .Include(vs => vs.ChildVideoStreams)
                .ThenInclude(vsl => vsl.ChildVideoStream)
            .Include(vs => vs.ParentVideoStreams)
                .ThenInclude(vsl => vsl.ParentVideoStream)
            .SingleOrDefaultAsync(vs => vs.Id == videoStreamId);
    }
}
