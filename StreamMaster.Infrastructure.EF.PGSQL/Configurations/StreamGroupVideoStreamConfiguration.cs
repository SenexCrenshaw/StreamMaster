using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace StreamMaster.Infrastructure.EF.PGSQL.Configurations;

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
            .OnDelete(DeleteBehavior.Cascade);

    }
}
