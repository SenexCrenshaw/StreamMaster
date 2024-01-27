using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StreamMaster.Infrastructure.EF.Configurations;

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
            .OnDelete(DeleteBehavior.Cascade);
    }
}
