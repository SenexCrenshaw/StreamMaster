using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StreamMasterDomain.Entities;

namespace StreamMasterInfrastructure.Persistence.Configurations;

public class StreamGroupChannelGroupConfiguration : IEntityTypeConfiguration<StreamGroupChannelGroup>
{
    public void Configure(EntityTypeBuilder<StreamGroupChannelGroup> modelBuilder)
    {
        modelBuilder
              .HasKey(sgvs => new { sgvs.ChannelGroupId, sgvs.StreamGroupId });

        modelBuilder
            .HasOne(sgcg => sgcg.StreamGroup)
            .WithMany(sg => sg.ChannelGroups)
            .HasForeignKey(sgcg => sgcg.StreamGroupId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

        //modelBuilder
        //    .HasOne(sgcg => sgcg.ChannelGroup)
        //    .WithMany(cg => cg.StreamGroups)
        //    .HasForeignKey(sgcg => sgcg.ChannelGroupId)
        //    .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
    }
}

public class StreamGroupVideoStreamConfiguration : IEntityTypeConfiguration<StreamGroupVideoStream>
{
    public void Configure(EntityTypeBuilder<StreamGroupVideoStream> modelBuilder)
    {
        modelBuilder
              .HasKey(sgvs => new { sgvs.ChildVideoStreamId, sgvs.StreamGroupId });

        modelBuilder
            .HasOne(sgvs => sgvs.ChildVideoStream)
            .WithMany(vs => vs.StreamGroups)
            .HasForeignKey(sgvs => sgvs.ChildVideoStreamId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

        //modelBuilder
        //    .HasOne(sgvs => sgvs.StreamGroup)
        //.WithMany(sg => sg.ChildVideoStreams)
        //.HasForeignKey(sgvs => sgvs.StreamGroupId)
        //.OnDelete(DeleteBehavior.Cascade);  // Add this line
    }
}

public class VideoStreamLinkConfiguration : IEntityTypeConfiguration<VideoStreamLink>
{
    public void Configure(EntityTypeBuilder<VideoStreamLink> modelBuilder)
    {
        modelBuilder.HasKey(vsl => new { vsl.ParentVideoStreamId, vsl.ChildVideoStreamId }); // Composite key

        modelBuilder.HasOne(vsl => vsl.ParentVideoStream)
            .WithMany(vs => vs.ChildVideoStreams)
            .HasForeignKey(vsl => vsl.ParentVideoStreamId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

        //modelBuilder.HasOne(vsl => vsl.ChildVideoStream)
        //    .WithMany(vs => vs.ParentVideoStreams)
        //    .HasForeignKey(vsl => vsl.ChildVideoStreamId)
        //    .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
    }
}
