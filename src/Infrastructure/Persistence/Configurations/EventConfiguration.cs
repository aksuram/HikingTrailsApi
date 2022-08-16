using HikingTrailsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HikingTrailsApi.Infrastructure.Persistence.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.Description)
                .IsRequired();


            builder.Property(x => x.CreationDate)
                .IsRequired();


            builder.Property(x => x.UserId)
                .IsRequired();
        }
    }
}
