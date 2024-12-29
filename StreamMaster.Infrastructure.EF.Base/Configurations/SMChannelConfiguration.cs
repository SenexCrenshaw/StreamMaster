using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StreamMaster.Infrastructure.EF.Base.Configurations;

public class SMChannelConfiguration : IEntityTypeConfiguration<SMChannel>
{
    public void Configure(EntityTypeBuilder<SMChannel> modelBuilder)
    {
        modelBuilder.HasIndex(e => new { e.BaseStreamID, e.M3UFileId })
                  .IsUnique()
                  .HasDatabaseName("smchannels_unique"); 
      
    }
}
