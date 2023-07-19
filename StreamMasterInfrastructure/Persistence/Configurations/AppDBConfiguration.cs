using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StreamMasterDomain.Entities;

namespace StreamMasterInfrastructure.Persistence.Configurations;

public class StreamGroupConfiguration : IEntityTypeConfiguration<StreamGroup>
{
    public void Configure(EntityTypeBuilder<StreamGroup> modelBuilder)
    {
        modelBuilder
         .HasMany(e => e.VideoStreams)
         .WithMany(e => e.StreamGroups)
         .UsingEntity<StreamGroupVideoStream>();

        modelBuilder
        .HasMany(e => e.ChannelGroups)
        .WithMany(e => e.StreamGroups)
        .UsingEntity<StreamGroupChannelGroup>();
    }
}

public class VideoStreamConfiguration : IEntityTypeConfiguration<VideoStreamLink>
{
    public void Configure(EntityTypeBuilder<VideoStreamLink> modelBuilder)
    {
        modelBuilder.HasKey(vsl => new { vsl.ParentVideoStreamId, vsl.ChildVideoStreamId }); // Composite key

        modelBuilder.HasOne(vsl => vsl.ParentVideoStream)
            .WithMany(vs => vs.ChildVideoStreams)
            .HasForeignKey(vsl => vsl.ParentVideoStreamId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

        modelBuilder.HasOne(vsl => vsl.ChildVideoStream)
            .WithMany(vs => vs.ParentVideoStreams)
            .HasForeignKey(vsl => vsl.ChildVideoStreamId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
    }
}
