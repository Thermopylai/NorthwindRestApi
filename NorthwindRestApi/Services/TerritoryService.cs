using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Common;
using NorthwindRestApi.Data;
using NorthwindRestApi.DTOs.Territories;
using NorthwindRestApi.Extensions;
using NorthwindRestApi.Models;
using NorthwindRestApi.Models.Entities;
using NorthwindRestApi.Projections;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Services
{
    public class TerritoryService : ITerritoryService
    {
        private readonly NorthwindOriginalContext _db;

        public TerritoryService(NorthwindOriginalContext db)
        {
            _db = db;
        }

        public async Task<List<TerritoryReadDto>> GetAllAsync(CancellationToken ct)
        {
            return await BuildTerritoryReadQuery()
                .OrderBy(t => t.TerritoryID)
                .ToListAsync(ct);
        }

        public async Task<TerritoryReadDto?> GetByIdAsync(string id, CancellationToken ct)
        {
            return await BuildTerritoryReadQuery()
                .FirstOrDefaultAsync(t => t.TerritoryID == id, ct);
        }

        public async Task<PagedResult<TerritoryReadDto>> GetPagedAsync(
            int page, 
            int pageSize, 
            CancellationToken ct)
        {
            return await BuildTerritoryReadQuery()
                .OrderBy(t => t.TerritoryID)
                .ToPagedResultAsync(page, pageSize, ct);
        }
        
        public async Task<PagedResult<TerritoryReadDto>> SearchAsync(
            TerritoryQueryParameters parameters, 
            CancellationToken ct)
        {
            var query = BuildTerritoryReadQuery()
                .ApplyFilter(parameters)
                .ApplySearch(parameters.SearchTerm)
                .ApplySorting(parameters.OrderBy, parameters.Descending);

            return await query.ToPagedResultAsync(parameters.Page, parameters.PageSize, ct);
        }

        public async Task<TerritoryReadDto> CreateAsync(TerritoryCreateDto dto, CancellationToken ct)
        {
            var entity = new Territory
            {
                TerritoryID = dto.TerritoryID,
                TerritoryDescription = dto.TerritoryDescription,
                RegionID = dto.RegionID,
                IsDeleted = dto.IsDeleted
            };

            _db.Territories.Add(entity);
            await _db.SaveChangesAsync(ct);

            return await TerritoryReadProjections.Build(_db.Territories.AsNoTracking())
                .FirstAsync(t => t.TerritoryID == entity.TerritoryID, ct);
        }

        public async Task<TerritoryReadDto?> UpdateAsync(string id, TerritoryUpdateDto dto, CancellationToken ct)
        {
            var entity = await _db.Territories.FindAsync(new object[] { id }, ct);

            if (entity == null)
                return null;

            entity.TerritoryDescription = dto.TerritoryDescription;
            entity.RegionID = dto.RegionID;
            entity.IsDeleted = dto.IsDeleted;

            await _db.SaveChangesAsync(ct);

            return await TerritoryReadProjections.Build(_db.Territories.AsNoTracking())
                .FirstAsync(t => t.TerritoryID == entity.TerritoryID, ct);
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken ct)
        {
            var affected = await _db.Territories
                .Where(p => p.TerritoryID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(p => p.IsDeleted, true), ct);

            return affected > 0;
        }

        private IQueryable<TerritoryReadDto> BuildTerritoryReadQuery()
        {
            return TerritoryReadProjections.Build(
                _db.Territories.AsNoTracking());
        }
    }
}
