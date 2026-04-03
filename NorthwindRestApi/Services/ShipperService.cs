using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Common;
using NorthwindRestApi.Data;
using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.DTOs.Shippers;
using NorthwindRestApi.Extensions;
using NorthwindRestApi.Models;
using NorthwindRestApi.Models.Entities;
using NorthwindRestApi.Projections;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Services
{
    public class ShipperService : IShipperService
    {
        private readonly NorthwindOriginalContext _db;

        public ShipperService(NorthwindOriginalContext db)
        {
            _db = db;
        }

        public async Task<List<ShipperListDto>> GetAllAsync(CancellationToken ct)
        {
            return await BuildShipperListQuery()
                .OrderBy(s => s.ShipperID)
                .ToListAsync(ct);
        }

        public async Task<ShipperReadDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await BuildShipperReadQuery()
                .FirstOrDefaultAsync(s => s.ShipperID == id, ct);
        }

        public async Task<PagedResult<ShipperListDto>> GetPagedAsync(
            int page, 
            int pageSize, 
            CancellationToken ct)
        {
            return await BuildShipperListQuery()
                .OrderBy(s => s.ShipperID)
                .ToPagedResultAsync(page, pageSize, ct);
        }

        public async Task<PagedResult<ShipperListDto>> SearchAsync(
            ShipperQueryParameters parameters, 
            CancellationToken ct)
        {
            var query = BuildShipperListQuery()
                .ApplyFilter(parameters)
                .ApplySearch(parameters.SearchTerm)
                .ApplySorting(parameters.OrderBy, parameters.Descending);

            return await query.ToPagedResultAsync(parameters.Page, parameters.PageSize, ct);
        }

        public async Task<ShipperReadDto> CreateAsync(ShipperCreateDto dto, CancellationToken ct)
        {
            var entity = new Shipper
            {
                CompanyName = dto.CompanyName,
                Phone = dto.Phone,
                RegionID = dto.RegionID,
                IsDeleted = dto.IsDeleted
            };

            _db.Shippers.Add(entity);
            await _db.SaveChangesAsync(ct);

            return await ShipperReadProjections.Build(
                _db.Shippers.AsNoTracking()
                    .Where(s => s.ShipperID == entity.ShipperID),
                OrderReadProjections.Build(
                    _db.Orders.AsNoTracking()))
                    .FirstAsync(ct);
        }

        public async Task<ShipperReadDto?> UpdateAsync(int id, ShipperUpdateDto dto, CancellationToken ct)
        {
            var entity = await _db.Shippers.FindAsync(new object[] { id }, ct);

            if (entity == null)
                return null;

            entity.CompanyName = dto.CompanyName;
            entity.Phone = dto.Phone;
            entity.RegionID = dto.RegionID;
            entity.IsDeleted = dto.IsDeleted;

            await _db.SaveChangesAsync(ct);

            return await ShipperReadProjections.Build(
                _db.Shippers.AsNoTracking()
                    .Where(s => s.ShipperID == entity.ShipperID),
                OrderReadProjections.Build(
                    _db.Orders.AsNoTracking()))
                    .FirstAsync(ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var affected = await _db.Shippers
                .Where(p => p.ShipperID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(p => p.IsDeleted, true), ct);

            return affected > 0;
        }

        private IQueryable<ShipperListDto> BuildShipperListQuery()
        {
            return ShipperListProjections.Build(
                _db.Shippers.AsNoTracking());
        }

        private IQueryable<ShipperReadDto> BuildShipperReadQuery()
        {
            return ShipperReadProjections.Build(
                _db.Shippers.AsNoTracking(),
                OrderReadProjections.Build(
                    _db.Orders.AsNoTracking()));
        }

    }
}
