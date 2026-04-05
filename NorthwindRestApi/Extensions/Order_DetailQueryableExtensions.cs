using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.DTOs.Order_Details;
using NorthwindRestApi.DTOs.Orders;

namespace NorthwindRestApi.Extensions
{
    public static class Order_DetailQueryableExtensions
    {
        public static IQueryable<Order_DetailReadDto> ApplyFilter(
            this IQueryable<Order_DetailReadDto> query,
            Order_DetailQueryParameters parameters)
        {
            query = query.IgnoreQueryFilters();

            if (parameters.OrderId.HasValue)
            {
                query = query.Where(p => p.OrderID == parameters.OrderId.Value);
            }

            if (parameters.ProductId.HasValue)
            {
                query = query.Where(p => p.ProductID == parameters.ProductId.Value);
            }

            if (parameters.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryID == parameters.CategoryId.Value);
            }

            if (parameters.SupplierId.HasValue)
            {
                query = query.Where(p => p.SupplierID == parameters.SupplierId.Value);
            }

            if (parameters.EmployeeId.HasValue)
            {
                query = query.Where(p => p.EmployeeID == parameters.EmployeeId.Value);
            }

            if (parameters.ShipVia.HasValue)
            {
                query = query.Where(p => p.ShipVia == parameters.ShipVia.Value);
            }

            if (parameters.MinTotalPrice.HasValue)
            {
                query = query.Where(p => p.TotalPrice.HasValue && p.TotalPrice.Value >= parameters.MinTotalPrice.Value);
            }

            if (parameters.MaxTotalPrice.HasValue)
            {
                query = query.Where(p => p.TotalPrice.HasValue && p.TotalPrice.Value <= parameters.MaxTotalPrice.Value);
            }

            if (parameters.IsDeleted.HasValue)
            {
                query = query.Where(p => p.IsDeleted == parameters.IsDeleted.Value);
            }

            if (parameters.VatRate.HasValue)
            {
                query = query.Where(p => p.VatRate == parameters.VatRate.Value);
            }

            return query;
        }
        public static IQueryable<Order_DetailReadDto> ApplySearch(
            this IQueryable<Order_DetailReadDto> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var term = searchTerm.Trim();

            return query.Where(o =>
                (o.ProductName != null && o.ProductName.Contains(term)) ||
                (o.CategoryName != null && o.CategoryName.Contains(term)) ||
                (o.SupplierName != null && o.SupplierName.Contains(term)) ||
                (o.CustomerCompanyName != null && o.CustomerCompanyName.Contains(term)) ||
                (o.EmployeeFullName != null && o.EmployeeFullName.Contains(term)) ||
                (o.ShipViaCompanyName != null && o.ShipViaCompanyName.Contains(term)));
        }

        public static IQueryable<Order_DetailReadDto> ApplySorting(
            this IQueryable<Order_DetailReadDto> query,
            string? orderBy,
            bool descending)
        {
            var key = orderBy?.Trim().ToLowerInvariant();

            return key switch
            {
                "orderid" => descending
                    ? query.OrderByDescending(od => od.OrderID)
                    : query.OrderBy(od => od.OrderID),

                "productid" => descending
                    ? query.OrderByDescending(od => od.ProductID)
                    : query.OrderBy(od => od.ProductID),

                "productname" => descending
                    ? query.OrderByDescending(od => od.ProductName)
                    : query.OrderBy(od => od.ProductName),

                "categoryid" => descending
                    ? query.OrderByDescending(od => od.CategoryID)
                    : query.OrderBy(od => od.CategoryID),

                "categoryname" => descending
                    ? query.OrderByDescending(od => od.CategoryName)
                    : query.OrderBy(od => od.CategoryName),

                "supplierid" => descending
                    ? query.OrderByDescending(od => od.SupplierID)
                    : query.OrderBy(od => od.SupplierID),

                "suppliername" => descending
                    ? query.OrderByDescending(od => od.SupplierName)
                    : query.OrderBy(od => od.SupplierName),

                "customerid" => descending
                    ? query.OrderByDescending(od => od.CustomerID)
                    : query.OrderBy(od => od.CustomerID),

                "employeeid" => descending
                    ? query.OrderByDescending(od => od.EmployeeID)
                    : query.OrderBy(od => od.EmployeeID),

                "employeefullname" => descending
                    ? query.OrderByDescending(od => od.EmployeeFullName)
                    : query.OrderBy(od => od.EmployeeFullName),

                "shipvia" => descending
                    ? query.OrderByDescending(od => od.ShipVia)
                    : query.OrderBy(od => od.ShipVia),

                "shipviacompanyname" => descending
                    ? query.OrderByDescending(od => od.ShipViaCompanyName)
                    : query.OrderBy(od => od.ShipViaCompanyName),

                "unitprice" => descending
                    ? query.OrderByDescending(od => od.UnitPrice)
                    : query.OrderBy(od => od.UnitPrice),

                "quantity" => descending
                    ? query.OrderByDescending(od => od.Quantity)
                    : query.OrderBy(od => od.Quantity),

                "discount" => descending
                    ? query.OrderByDescending(od => od.Discount)
                    : query.OrderBy(od => od.Discount),

                "totalprice" => descending
                    ? query.OrderByDescending(od => od.TotalPrice)
                    : query.OrderBy(od => od.TotalPrice),

                "pricewithdiscount" => descending
                    ? query.OrderByDescending(od => od.PriceWithDiscount)
                    : query.OrderBy(od => od.PriceWithDiscount),

                "vatamount" => descending
                    ? query.OrderByDescending(od => od.VatAmount)
                    : query.OrderBy(od => od.VatAmount),

                "pricewithvat" => descending
                    ? query.OrderByDescending(od => od.PriceWithVat)
                    : query.OrderBy(od => od.PriceWithVat),

                _ => query.OrderBy(od => od.OrderID).ThenBy(od => od.ProductID),
            };
        }
    }
}
