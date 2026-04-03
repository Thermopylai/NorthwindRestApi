using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Common;
using NorthwindRestApi.Data;
using NorthwindRestApi.DTOs.Order_Details;
using NorthwindRestApi.Extensions;
using NorthwindRestApi.Models;
using NorthwindRestApi.Models.Entities;
using NorthwindRestApi.Projections;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Services
{
    public class Order_DetailService : IOrder_DetailService
    {
        private readonly NorthwindOriginalContext _db;

        public Order_DetailService(NorthwindOriginalContext db)
        {
            _db = db;
        }

        public async Task<List<Order_DetailReadDto>> GetAllAsync(CancellationToken ct)
        {
            return await BuildOrder_DetailReadQuery()
                .OrderBy(od => od.OrderID)
                .ThenBy(od => od.ProductID)
                .ToListAsync(ct);
        }

        public async Task<Order_DetailReadDto?> GetByIdAsync(int orderId, int productId, CancellationToken ct)
        {
            return await BuildOrder_DetailReadQuery()
                .FirstOrDefaultAsync(od => od.OrderID == orderId && od.ProductID == productId, ct);
        }

        public async Task<List<Order_DetailReadDto>> GetByOrderIdAsync(int orderId, CancellationToken ct)
        {
            return await BuildOrder_DetailReadQuery()
                .Where(od => od.OrderID == orderId)
                .OrderBy(od => od.ProductID)
                .ToListAsync(ct);
        }

        public async Task<List<Order_DetailReadDto>> GetByProductIdAsync(int productId, CancellationToken ct)
        {
            return await BuildOrder_DetailReadQuery()
                .Where(od => od.ProductID == productId)
                .OrderBy(od => od.OrderID)
                .ToListAsync(ct);
        }

        public async Task<PagedResult<Order_DetailReadDto>> GetPagedAsync(
            int page, 
            int pageSize, 
            CancellationToken ct)
        {
            return await BuildOrder_DetailReadQuery()
                .OrderBy(od => od.OrderID)
                .ThenBy(od => od.ProductID)
                .ToPagedResultAsync(page, pageSize, ct);
        }

        public async Task<PagedResult<Order_DetailReadDto>> SearchAsync(
            Order_DetailQueryParameters parameters,
            CancellationToken ct)
        {
            var query = BuildOrder_DetailReadQuery()
                .ApplyFilter(parameters)
                .ApplySearch(parameters.SearchTerm)
                .ApplySorting(parameters.OrderBy, parameters.Descending);

            return await query.ToPagedResultAsync(parameters.Page, parameters.PageSize, ct);
        }

        public async Task<Order_DetailReadDto> CreateAsync(Order_DetailCreateDto dto, CancellationToken ct)
        {
            var entity = new Order_Detail
            {
                OrderID = dto.OrderID,
                ProductID = dto.ProductID,
                UnitPrice = dto.UnitPrice,
                Quantity = dto.Quantity,
                Discount = dto.Discount,
                IsDeleted = dto.IsDeleted
            };

            _db.Order_Details.Add(entity);
            await _db.SaveChangesAsync(ct);

            return await Order_DetailReadProjections.Build(_db.Order_Details.AsNoTracking())
                .FirstAsync(od => od.OrderID == entity.OrderID && od.ProductID == entity.ProductID, ct);
        }

        public async Task<Order_DetailReadDto?> UpdateAsync(int orderId, int productId, Order_DetailUpdateDto dto, CancellationToken ct)
        {
            var entity = await _db.Order_Details
                .FirstOrDefaultAsync(od => od.OrderID == orderId && od.ProductID == productId, ct);

            if (entity == null)
                return null;
            

            entity.Quantity = dto.Quantity;
            entity.Discount = dto.Discount;
            entity.IsDeleted = dto.IsDeleted;

            await _db.SaveChangesAsync(ct);

            return await Order_DetailReadProjections.Build(_db.Order_Details.AsNoTracking())
                .FirstAsync(od => od.OrderID == entity.OrderID && od.ProductID == entity.ProductID, ct);
        }

        public async Task<bool> DeleteAsync(int orderId, int productId, CancellationToken ct)
        {
            var affected = await _db.Order_Details
                .Where(od => od.OrderID == orderId && od.ProductID == productId)
                .ExecuteUpdateAsync(u => u.SetProperty(od => od.IsDeleted,true), ct);

            return affected > 0;
        }

        private IQueryable<Order_DetailReadDto> BuildOrder_DetailReadQuery()
        {
            return Order_DetailReadProjections.Build(_db.Order_Details.AsNoTracking());
        }
    }
}
