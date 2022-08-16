using HikingTrailsApi.Application.Common.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Common.Mappings
{
    public static class MappingExtensions
    {
        public static Task<PaginatedList<T>> PaginatedListAsync<T>(this IQueryable<T> queryable, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return PaginatedList<T>.CreateAsync(queryable, pageNumber, pageSize, cancellationToken);
        }
    }
}
