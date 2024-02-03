using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StreamMaster.Infrastructure.EF.PSQL.Configurations;

public class VideoStreamLinkConfiguration : IEntityTypeConfiguration<VideoStreamLink>
{
    public void Configure(EntityTypeBuilder<VideoStreamLink> modelBuilder)
    {
        modelBuilder.HasKey(vsl => new { vsl.ParentVideoStreamId, vsl.ChildVideoStreamId });

        modelBuilder.HasOne(vsl => vsl.ParentVideoStream)
            .WithMany(vs => vs.ChildVideoStreams)
            .HasForeignKey(vsl => vsl.ParentVideoStreamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
