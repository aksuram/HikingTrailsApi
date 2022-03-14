using HikingTrailsApi.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HikingTrailsApi.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IApplicationDbContextFactory<IApplicationDbContext>
    {
        private readonly DbContextOptions _options;

        public ApplicationDbContextFactory(DbContextOptions options)
        {
            _options = options;
        }

        public IApplicationDbContext CreateDbContext()
        {
            return new ApplicationDbContext(_options);
        }
    }
}
