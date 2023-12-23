using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StreamMasterInfrastructureEF.Configurations;

public class VideoStreamLinkConfiguration : IEntityTypeConfiguration<VideoStreamLink>
{
    public void Configure(EntityTypeBuilder<VideoStreamLink> modelBuilder)
    {
        modelBuilder.HasKey(vsl => new { vsl.ParentVideoStreamId, vsl.ChildVideoStreamId }); // Composite key

        modelBuilder.HasOne(vsl => vsl.ParentVideoStream)
            .WithMany(vs => vs.ChildVideoStreams)
            .HasForeignKey(vsl => vsl.ParentVideoStreamId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete


    }
}
