using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StreamMaster.Infrastructure.EF.Base.Configurations;

public class StreamGroupSMChannelConfiguration : IEntityTypeConfiguration<StreamGroupSMChannel>
{
    public void Configure(EntityTypeBuilder<StreamGroupSMChannel> modelBuilder)
    {
        modelBuilder
              .HasKey(sgvs => new { sgvs.SMChannelId, sgvs.StreamGroupId });

        modelBuilder
            .HasOne(sgvs => sgvs.SMChannel)
            .WithMany(vs => vs.StreamGroups)
            .HasForeignKey(sgvs => sgvs.SMChannelId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
