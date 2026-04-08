using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Products;

namespace NorthwindRestApi.Extensions
{
    public static class ProductQueryableExtensions
    {
        /// <summary>
        /// Filters the product query based on the specified parameters, including supplier ID, category ID, discontinued status,
        /// minimum price, maximum price and VAT rate. Each filter is applied only if the corresponding parameter has a value. The resulting
        /// query will include only products that match all of the specified criteria. If a parameter is null, it is ignored and does not affect the filtering.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns>An IQueryable containing only the products that match all of the specified filter parameters.</returns>
        public static IQueryable<ProductListDto> ApplyFilter(
            this IQueryable<ProductListDto> query,
            ProductQueryParameters parameters)
        {
            query = query.IgnoreQueryFilters();

            if (parameters.SupplierId.HasValue)
            {
                query = query.Where(p => p.SupplierID == parameters.SupplierId.Value);
            }

            if (parameters.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryID == parameters.CategoryId.Value);
            }

            if (parameters.Discontinued.HasValue)
            {
                query = query.Where(p => p.Discontinued == parameters.Discontinued.Value);
            }

            if (parameters.MinPrice.HasValue)
            {
                query = query.Where(p => p.UnitPrice.HasValue &&
                                         p.UnitPrice.Value >= parameters.MinPrice.Value);
            }

            if (parameters.MaxPrice.HasValue)
            {
                query = query.Where(p => p.UnitPrice.HasValue &&
                                         p.UnitPrice.Value <= parameters.MaxPrice.Value);
            }

            if (parameters.VatRate.HasValue)
            {
                if (parameters.VatRate.Value == VatRules.ReducedVatRate)
                {
                    query = query.Where(p =>
                        p.CategoryName != null &&
                        VatRules.ReducedVatCategories.Contains(p.CategoryName));
                }
                else if (parameters.VatRate.Value == VatRules.StandardVatRate)
                {
                    query = query.Where(p =>
                        p.CategoryName == null ||
                        !VatRules.ReducedVatCategories.Contains(p.CategoryName));
                }
                else
                {
                    query = query.Where(p => false);
                }
            }

            return query;
        }

        /// <summary>
        /// Filters the product query to include only products whose name, supplier, category, quantity per unit, or
        /// image link contains the specified search term.
        /// </summary>
        /// <remarks>The search is case-sensitive and matches partial values within the fields. If the
        /// search term is null or consists only of whitespace, the original query is returned unfiltered.</remarks>
        /// <param name="query">The source query of products to filter.</param>
        /// <param name="searchTerm">The search term to match against product fields. If null or whitespace, no filtering is applied.</param>
        /// <returns>An IQueryable containing products that match the search term in any of the specified fields.</returns>
        public static IQueryable<ProductListDto> ApplySearch(
            this IQueryable<ProductListDto> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var term = searchTerm.Trim();

            return query.Where(p =>
                (p.ProductName != null && p.ProductName.Contains(term)) ||
                (p.SupplierName != null && p.SupplierName.Contains(term)) ||
                (p.CategoryName != null && p.CategoryName.Contains(term)) ||
                (p.QuantityPerUnit != null && p.QuantityPerUnit.Contains(term)) ||
                (p.ImageLink != null && p.ImageLink.Contains(term)));
        }

        /// <summary>
        /// Applies dynamic sorting to a sequence of ProductReadDto items based on the specified property and sort
        /// direction.
        /// </summary>
        /// <remarks>The sorting is case-insensitive and defaults to ascending order by ProductID if the
        /// property name is not recognized. This method is intended for use with LINQ providers that support expression
        /// trees, such as Entity Framework.</remarks>
        /// <param name="query">The source queryable collection of ProductReadDto items to sort.</param>
        /// <param name="orderBy">The name of the property to sort by. Supported values include "ProductID", "ProductName", "SupplierID",
        /// "SupplierName", "CategoryID", "CategoryName", "UnitPrice", "UnitsInStock", "UnitsOnOrder", "ReorderLevel",
        /// "ImageLink", "VatAmount", "PriceWithVat", and "StockValueWithVat". If null or unrecognized, sorting defaults
        /// to "ProductID".</param>
        /// <param name="descending">A value indicating whether to sort in descending order. If <see langword="true"/>, the results are sorted in
        /// descending order; otherwise, in ascending order.</param>
        /// <returns>An IQueryable of ProductReadDto items sorted according to the specified property and direction.</returns>
        public static IQueryable<ProductListDto> ApplySorting(
            this IQueryable<ProductListDto> query,
            string? orderBy,
            bool descending)
        {
            var key = orderBy?.Trim().ToLowerInvariant();

            return key switch
            {
                "productid" => descending
                    ? query.OrderByDescending(p => p.ProductID)
                    : query.OrderBy(p => p.ProductID),

                "productname" => descending
                    ? query.OrderByDescending(o => o.ProductName)
                    : query.OrderBy(p => p.ProductName),
                
                "supplierid" => descending
                    ? query.OrderByDescending(o => o.SupplierID)
                    : query.OrderBy(p => p.SupplierID),

                "suppliername" => descending
                    ? query.OrderByDescending(o => o.SupplierName)
                    : query.OrderBy(p => p.SupplierName),

                "categoryid" => descending
                    ? query.OrderByDescending(o => o.CategoryID)
                    : query.OrderBy(p => p.CategoryID),

                "categoryname" => descending
                    ? query.OrderByDescending(o => o.CategoryName)
                    : query.OrderBy(p => p.CategoryName),

                "unitprice" => descending
                    ? query.OrderByDescending(o => o.UnitPrice)
                    : query.OrderBy(p => p.UnitPrice),

                "unitsinstock" => descending
                    ? query.OrderByDescending(o => o.UnitsInStock)
                    : query.OrderBy(p => p.UnitsInStock),

                "unitsonorder" => descending
                    ? query.OrderByDescending(o => o.UnitsOnOrder)
                    : query.OrderBy(p => p.UnitsOnOrder),

                "reorderlevel" => descending
                    ? query.OrderByDescending(o => o.ReorderLevel)
                    : query.OrderBy(p => p.ReorderLevel),

                "imagelink" => descending
                    ? query.OrderByDescending(o => o.ImageLink)
                    : query.OrderBy(p => p.ImageLink),
                
                "vatamount" => descending
                    ? query.OrderByDescending(o => o.VatAmount)
                    : query.OrderBy(p => p.VatAmount),

                "pricewithvat" => descending
                    ? query.OrderByDescending(o => o.PriceWithVat)
                    : query.OrderBy(p => p.PriceWithVat),

                "stockvalue" => descending
                    ? query.OrderByDescending(p => p.StockValue)
                    : query.OrderBy(p => p.StockValue),

                "stockvaluewithvat" => descending
                    ? query.OrderByDescending(p => p.StockValueWithVat)
                    : query.OrderBy(p => p.StockValueWithVat),

                _ => query.OrderBy(p => p.ProductID)
            };
        }
    }
}

