using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Common;
using NorthwindRestApi.Data;
using NorthwindRestApi.DTOs.Categories;
using NorthwindRestApi.DTOs.Regions;
using NorthwindRestApi.Extensions;
using NorthwindRestApi.Models;
using NorthwindRestApi.Models.Entities;
using NorthwindRestApi.Projections;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Services
{
    public class RegionService : IRegionService
    {
        private readonly NorthwindOriginalContext _db;

        public RegionService(NorthwindOriginalContext db)
        {
            _db = db;
        }

        public async Task<List<RegionListDto>> GetAllAsync(CancellationToken ct)
        {
            return await BuildRegionListQuery()
                .OrderBy(r => r.RegionID)
                .ToListAsync(ct);
        }

        public async Task<RegionReadDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await BuildRegionReadQuery()
                .FirstOrDefaultAsync(r => r.RegionID == id, ct);
        }

        public async Task<PagedResult<RegionListDto>> GetPagedAsync(
            int page, 
            int pageSize, 
            CancellationToken ct)
        {
            return await BuildRegionListQuery()
                .OrderBy(r => r.RegionID)
                .ToPagedResultAsync(page, pageSize, ct);
        }

        public async Task<PagedResult<RegionListDto>> SearchAsync(
            RegionQueryParameters parameters,
            CancellationToken ct)
        {
            var query = BuildRegionListQuery()
                .ApplyFilter(parameters)
                .ApplySearch(parameters.SearchTerm)
                .ApplySorting(parameters.OrderBy, parameters.Descending);

            return await query.ToPagedResultAsync(parameters.Page, parameters.PageSize, ct);
        }

        public async Task<RegionReadDto> CreateAsync(RegionCreateDto dto, CancellationToken ct)
        {
            var entity = new Region
            {
                RegionDescription = dto.RegionDescription,
                IsDeleted = dto.IsDeleted
            };

            _db.Regions.Add(entity);
            await _db.SaveChangesAsync(ct);

            return await RegionReadProjections.Build(
                _db.Regions.AsNoTracking()
                    .Where(r => r.RegionID == entity.RegionID),
                TerritoryReadProjections.Build(
                    _db.Territories.AsNoTracking()),
                ShipperReadProjections.Build(
                    _db.Shippers.AsNoTracking(),
                    OrderReadProjections.Build(
                        _db.Orders.AsNoTracking())))
                    .FirstAsync(ct);
        }

        public async Task<RegionReadDto?> UpdateAsync(int id, RegionUpdateDto dto, CancellationToken ct)
        {
            var entity = await _db.Regions.FindAsync(new object[] { id }, ct);

            if (entity == null)
                return null;
            
            entity.RegionDescription = dto.RegionDescription;
            entity.IsDeleted = dto.IsDeleted;

            await _db.SaveChangesAsync(ct);

            return await RegionReadProjections.Build(
                _db.Regions.AsNoTracking()
                    .Where(r => r.RegionID == entity.RegionID),
                TerritoryReadProjections.Build(
                    _db.Territories.AsNoTracking()),
                ShipperReadProjections.Build(
                    _db.Shippers.AsNoTracking(),
                    OrderReadProjections.Build(
                        _db.Orders.AsNoTracking())))
                    .FirstAsync(ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var affected = await _db.Regions
                .Where(p => p.RegionID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(p => p.IsDeleted, true), ct);

            return affected > 0;
        }

        private IQueryable<RegionListDto> BuildRegionListQuery()
        {
            return RegionListProjections.Build(
                _db.Regions.AsNoTracking(),
                TerritoryReadProjections.Build(
                    _db.Territories.AsNoTracking()),
                ShipperListProjections.Build(
                    _db.Shippers.AsNoTracking()));
        }

        private IQueryable<RegionReadDto> BuildRegionReadQuery()
        {
            return RegionReadProjections.Build(
                _db.Regions.AsNoTracking(),
                TerritoryReadProjections.Build(
                    _db.Territories.AsNoTracking()),
                ShipperReadProjections.Build(
                    _db.Shippers.AsNoTracking(),
                    OrderReadProjections.Build(
                        _db.Orders.AsNoTracking())));
        }
    }
}
