using HikingTrailsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HikingTrailsApi.Infrastructure.Persistence.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.Body)
                .IsRequired();


            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.Property(x => x.CreationDate)
                .IsRequired();


            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.PostId)
                .IsRequired();
        }
    }
}
