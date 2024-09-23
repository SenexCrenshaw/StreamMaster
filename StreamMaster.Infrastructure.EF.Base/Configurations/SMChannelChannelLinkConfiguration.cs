using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StreamMaster.Infrastructure.EF.Base.Configurations;

public class SMChannelChannelLinkConfiguration : IEntityTypeConfiguration<SMChannelChannelLink>
{
    public void Configure(EntityTypeBuilder<SMChannelChannelLink> modelBuilder)
    {
        modelBuilder.HasKey(vsl => new { vsl.ParentSMChannelId, vsl.ChildSMChannelId });

        modelBuilder.HasOne(vsl => vsl.ParentSMChannel)
            .WithMany(vs => vs.SMChannels)
            .HasForeignKey(vsl => vsl.ParentSMChannelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

