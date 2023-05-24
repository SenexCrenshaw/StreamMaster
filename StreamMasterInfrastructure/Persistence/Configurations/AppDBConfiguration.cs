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
    }
}

public class VideoStreamConfiguration : IEntityTypeConfiguration<VideoStreamRelationship>
{
    public void Configure(EntityTypeBuilder<VideoStreamRelationship> modelBuilder)
    {
        modelBuilder
        .HasKey(r => new { r.ParentVideoStreamId, r.ChildVideoStreamId });

        modelBuilder
       .HasOne(cr => cr.ParentVideoStream)
        .WithMany(vs => vs.ParentRelationships)
        .HasForeignKey(cr => cr.ParentVideoStreamId)
        .OnDelete(DeleteBehavior.NoAction);

        modelBuilder
        .HasOne(cr => cr.ChildVideoStream)
        .WithMany(vs => vs.ChildRelationships)
        .HasForeignKey(cr => cr.ChildVideoStreamId)
        .OnDelete(DeleteBehavior.NoAction);
    }
}
