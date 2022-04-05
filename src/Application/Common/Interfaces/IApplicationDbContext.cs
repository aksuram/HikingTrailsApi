using HikingTrailsApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Common.Interfaces
{
    public interface IApplicationDbContext : IDisposable
    {
        DbSet<User> Users { get; set; }
        DbSet<Event> Events { get; set; }
        DbSet<Post> Posts { get; set; }
        DbSet<Comment> Comments { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        int SaveChanges();
    }
}
