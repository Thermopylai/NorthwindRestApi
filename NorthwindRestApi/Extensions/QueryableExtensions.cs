using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.DTOs.Products;

namespace NorthwindRestApi.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            // Validointi
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            if (pageSize > 100)
                pageSize = 100;

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
