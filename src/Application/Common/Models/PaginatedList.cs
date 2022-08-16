using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HikingTrailsApi.Application.Common.Models
{
    public class PaginatedList<T>
    {
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalPageCount { get; }
        public int TotalItemCount { get; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPageCount;

        public List<T> Items { get; }

        public PaginatedList()
        {
        }

        public PaginatedList(List<T> items, int itemCount, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPageCount = (int)Math.Ceiling(itemCount / (double)pageSize);
            TotalItemCount = itemCount;
            Items = items;
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> query, int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            var itemCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<T>(items, itemCount, pageIndex, pageSize);
        }
    }
}
