using HikingTrailsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HikingTrailsApi.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.Email)
                .IsRequired();

            builder.Property(x => x.Password)
                .IsRequired();

            builder.Property(x => x.Role)
                .IsRequired();


            builder.Property(x => x.FirstName)
                .IsRequired();

            builder.Property(x => x.LastName)
                .IsRequired();


            builder.Property(x => x.IsEmailConfirmed)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.CreationDate)
                .IsRequired();
        }
    }
}
