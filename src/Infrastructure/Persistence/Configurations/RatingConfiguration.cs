using HikingTrailsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HikingTrailsApi.Infrastructure.Persistence.Configurations
{
    public class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.IsPositive)
                .IsRequired();


            builder.Property(x => x.CreationDate)
                .IsRequired();


            builder.Property(x => x.UserId)
                .IsRequired();
        }
    }
}
