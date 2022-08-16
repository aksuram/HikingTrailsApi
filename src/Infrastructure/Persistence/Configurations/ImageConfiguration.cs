using HikingTrailsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HikingTrailsApi.Infrastructure.Persistence.Configurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.Path)
                .IsRequired();


            builder.Property(x => x.CreationDate)
                .IsRequired();
        }
    }
}
