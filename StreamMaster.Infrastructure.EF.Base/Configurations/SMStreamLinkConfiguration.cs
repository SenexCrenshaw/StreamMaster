using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StreamMaster.Infrastructure.EF.Base.Configurations;

public class SMChannelStreamLinkConfiguration : IEntityTypeConfiguration<SMChannelStreamLink>
{
    public void Configure(EntityTypeBuilder<SMChannelStreamLink> entity)
    {
        entity.HasKey(vsl => new { vsl.SMChannelId, vsl.SMStreamId });

        entity.HasOne(vsl => vsl.SMChannel)
           .WithMany(vs => vs.SMStreams)
           .HasForeignKey(vsl => vsl.SMChannelId)
           .OnDelete(DeleteBehavior.Cascade);

    }
}


