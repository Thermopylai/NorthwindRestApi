using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Common;
using NorthwindRestApi.Data;
using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.Extensions;
using NorthwindRestApi.Models;
using NorthwindRestApi.Models.Entities;
using NorthwindRestApi.Projections;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly NorthwindOriginalContext _db;
        
        public OrderService(NorthwindOriginalContext db)
        {
            _db = db;
        }

        public async Task<List<OrderListDto>> GetAllAsync(CancellationToken ct)
        {
            return await BuildOrderListQuery()
                .OrderBy(o => o.OrderID)
                .ToListAsync(ct);
        }

        public async Task<OrderReadDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await BuildOrderReadQuery()
                .FirstOrDefaultAsync(o => o.OrderID == id, ct);
        }

        public async Task<List<OrderReadDto>> GetByCustomerIdAsync(string customerId, CancellationToken ct)
        {
            return await BuildOrderReadQuery()
                .Where(o => o.CustomerID == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.OrderID)
                .ToListAsync(ct);
        }

        public async Task<List<OrderReadDto>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken ct)
        {
            var startDate = DateTime.SpecifyKind(start.Date, DateTimeKind.Unspecified);
            var endDateExclusive = DateTime.SpecifyKind(end.Date.AddDays(1), DateTimeKind.Unspecified);

            return await BuildOrderReadQuery()
                .Where(o => o.OrderDate.HasValue &&
                            o.OrderDate.Value >= startDate &&
                            o.OrderDate.Value < endDateExclusive)
                .OrderByDescending(o => o.OrderDate)
                .ThenByDescending(o => o.OrderID)
                .ToListAsync(ct);
        }

        public async Task<PagedResult<OrderReadDto>> GetPagedAsync(
            int page, 
            int pageSize, 
            CancellationToken ct)
        {
            return await BuildOrderReadQuery()
                .OrderBy(o => o.OrderID)
                .ToPagedResultAsync(page, pageSize, ct);
        }

        public async Task<PagedResult<OrderReadDto>> SearchAsync(
            OrderQueryParameters parameters, 
            CancellationToken ct)
        {
            var query = BuildOrderReadQuery()
                .ApplyFilter(parameters)
                .ApplySearch(parameters.SearchTerm)
                .ApplySorting(parameters.OrderBy, parameters.Descending);

            return await query.ToPagedResultAsync(parameters.Page, parameters.PageSize, ct);
        }

        public async Task<OrderReadDto> CreateAsync(OrderCreateDto dto, CancellationToken ct)
        {
            var entity = new Order
            {
                CustomerID = dto.CustomerID,
                EmployeeID = dto.EmployeeID,
                OrderDate = dto.OrderDate,
                RequiredDate = dto.RequiredDate,
                ShippedDate = dto.ShippedDate,
                ShipVia = dto.ShipVia,
                Freight = dto.Freight,
                ShipName = dto.ShipName,
                ShipAddress = dto.ShipAddress,
                ShipCity = dto.ShipCity,
                ShipRegion = dto.ShipRegion,
                ShipPostalCode = dto.ShipPostalCode,
                ShipCountry = dto.ShipCountry,
                IsDeleted = dto.IsDeleted
            };

            _db.Orders.Add(entity);
            await _db.SaveChangesAsync(ct);

            return await OrderReadProjections.Build(_db.Orders.AsNoTracking())
                .FirstAsync(o => o.OrderID == entity.OrderID, ct);
        }

        public async Task<OrderReadDto?> UpdateAsync(int id, OrderUpdateDto dto, CancellationToken ct)
        {
            var entity = await _db.Orders.FindAsync(new object[] { id }, ct);

            if (entity == null)
                return null;

            entity.CustomerID = dto.CustomerID;
            entity.EmployeeID = dto.EmployeeID;
            entity.OrderDate = dto.OrderDate;
            entity.RequiredDate = dto.RequiredDate;
            entity.ShippedDate = dto.ShippedDate;
            entity.ShipVia = dto.ShipVia;
            entity.Freight = dto.Freight;
            entity.ShipName = dto.ShipName;
            entity.ShipAddress = dto.ShipAddress;
            entity.ShipCity = dto.ShipCity;
            entity.ShipRegion = dto.ShipRegion;
            entity.ShipPostalCode = dto.ShipPostalCode;
            entity.ShipCountry = dto.ShipCountry;
            entity.IsDeleted = dto.IsDeleted;

            await _db.SaveChangesAsync(ct);

            return await OrderReadProjections.Build(_db.Orders.AsNoTracking())
                .FirstAsync(o => o.OrderID == entity.OrderID, ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var affected = await _db.Orders
                .Where(o => o.OrderID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(o => o.IsDeleted, true), ct);

            return affected > 0;
        }

        public async Task<bool> RestoreAsync(int id, CancellationToken ct)
        {
            var affected = await _db.Orders
                .Where(o => o.OrderID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(o => o.IsDeleted, false), ct);
            return affected > 0;
        }

        private IQueryable<OrderListDto> BuildOrderListQuery()
        {
            return OrderListProjections.Build(_db.Orders.AsNoTracking());
        }

        private IQueryable<OrderReadDto> BuildOrderReadQuery()
        {
            return OrderReadProjections.Build(_db.Orders.AsNoTracking());
        }
    }
}