using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.DTOs.Categories;

namespace NorthwindRestApi.Extensions
{
    public static class CategoryQueryableExtensions
    {
        public static IQueryable<CategoryListDto> ApplyFilter(
                this IQueryable<CategoryListDto> query,
                CategoryQueryParameters parameters)
        {
            query = query.IgnoreQueryFilters();

            if (parameters.IsDeleted.HasValue)
            {
                query = query.Where(p => p.IsDeleted == parameters.IsDeleted.Value);
            }

            return query;
        }

        public static IQueryable<CategoryListDto> ApplySearch(
            this IQueryable<CategoryListDto> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var term = searchTerm.Trim();

            return query.Where(c =>
                (c.CategoryName != null && c.CategoryName.Contains(term)) ||
                (c.Description != null && c.Description.Contains(term)));
        }

        public static IQueryable<CategoryListDto> ApplySorting(
            this IQueryable<CategoryListDto> query,
            string? orderBy,
            bool descending)
        {
            var key = orderBy?.Trim().ToLowerInvariant();

            return key switch
            {
                "categoryid" => descending
                    ? query.OrderByDescending(c => c.CategoryID)
                    : query.OrderBy(c => c.CategoryID),

                "categoryname" => descending
                    ? query.OrderByDescending(c => c.CategoryName)
                    : query.OrderBy(c => c.CategoryName),

                "vatrate" => descending
                    ? query.OrderByDescending(c => c.VatRate)
                    : query.OrderBy(c => c.VatRate),

                "description" => descending
                    ? query.OrderByDescending(c => c.Description)
                    : query.OrderBy(c => c.Description),

                _ => query.OrderBy(c => c.CategoryID)
            };
        }
    }
}
