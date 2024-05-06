using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StreamMaster.Infrastructure.EF.PGSQL.Configurations;

public class StreamGroupSMChannelLinkConfiguration : IEntityTypeConfiguration<StreamGroupSMChannelLink>
{
    public void Configure(EntityTypeBuilder<StreamGroupSMChannelLink> modelBuilder)
    {
        modelBuilder
              .HasKey(sgvs => new { sgvs.StreamGroupId, sgvs.SMChannelId });

        modelBuilder.HasOne(vsl => vsl.StreamGroup)
             .WithMany(vs => vs.SMChannels)
             .HasForeignKey(vsl => vsl.StreamGroupId)
             .OnDelete(DeleteBehavior.Cascade);

    }
}
