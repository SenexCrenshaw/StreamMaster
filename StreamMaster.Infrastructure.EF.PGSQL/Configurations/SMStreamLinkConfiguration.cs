using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StreamMaster.Infrastructure.EF.PGSQL.Configurations;

public class SMStreamLinkConfiguration : IEntityTypeConfiguration<SMChannelStreamLink>
{
    public void Configure(EntityTypeBuilder<SMChannelStreamLink> modelBuilder)
    {
        modelBuilder.HasKey(vsl => new { vsl.SMChannelId, vsl.SMStreamId });

        modelBuilder.HasOne(vsl => vsl.SMChannel)
            .WithMany(vs => vs.SMStreams)
            .HasForeignKey(vsl => vsl.SMChannelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

