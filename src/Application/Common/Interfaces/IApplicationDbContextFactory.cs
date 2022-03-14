namespace HikingTrailsApi.Application.Common.Interfaces
{
    public interface IApplicationDbContextFactory<T> where T : IApplicationDbContext
    {
        public T CreateDbContext();
    }
}
