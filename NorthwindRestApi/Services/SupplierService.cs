using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Common;
using NorthwindRestApi.Data;
using NorthwindRestApi.DTOs.Suppliers;
using NorthwindRestApi.Extensions;
using NorthwindRestApi.Models;
using NorthwindRestApi.Models.Entities;
using NorthwindRestApi.Projections;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly NorthwindOriginalContext _db;

        public SupplierService(NorthwindOriginalContext db)
        {
            _db = db;
        }

        public async Task<List<SupplierListDto>> GetAllAsync(CancellationToken ct)
        {
            return await BuildSupplierListQuery()
                .OrderBy(s => s.SupplierID)
                .ToListAsync(ct);
        }

        public async Task<SupplierReadDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await BuildSupplierReadQuery()
                .FirstOrDefaultAsync(s => s.SupplierID == id, ct);
        }

        public async Task<PagedResult<SupplierReadDto>> GetPagedAsync(
            int page, 
            int pageSize, 
            CancellationToken ct)
        {
            return await BuildSupplierReadQuery()
                .OrderBy(s => s.SupplierID)
                .ToPagedResultAsync(page, pageSize, ct);
        }

        public async Task<PagedResult<SupplierReadDto>> SearchAsync(
            SupplierQueryParameters parameters, 
            CancellationToken ct)
        {
            var query = BuildSupplierReadQuery()
                .ApplyFilter(parameters)
                .ApplySearch(parameters.SearchTerm)
                .ApplySorting(parameters.OrderBy, parameters.Descending);

            return await query.ToPagedResultAsync(parameters.Page, parameters.PageSize, ct);
        }

        public async Task<SupplierReadDto> CreateAsync(SupplierCreateDto dto, CancellationToken ct)
        {
            var entity = new Supplier
            {
                CompanyName = dto.CompanyName,
                ContactName = dto.ContactName,
                ContactTitle = dto.ContactTitle,
                Address = dto.Address,
                City = dto.City,
                Region = dto.Region,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                Phone = dto.Phone,
                Fax = dto.Fax,
                HomePage = dto.HomePage,
                IsDeleted = dto.IsDeleted
            };

            _db.Suppliers.Add(entity);
            await _db.SaveChangesAsync(ct);

            return await SupplierReadProjections.Build(
                    _db.Suppliers.AsNoTracking()
                        .Where(s => s.SupplierID == entity.SupplierID),
                    ProductListProjections.Build(
                        _db.Products.AsNoTracking()))
                        .FirstAsync(ct);
        }

        public async Task<SupplierReadDto?> UpdateAsync(int id, SupplierUpdateDto dto, CancellationToken ct)
        {
            var entity = await _db.Suppliers.FindAsync(new object[] { id }, ct);

            if (entity == null)
                return null;

            entity.CompanyName = dto.CompanyName;
            entity.ContactName = dto.ContactName;
            entity.ContactTitle = dto.ContactTitle;
            entity.Address = dto.Address;
            entity.City = dto.City;
            entity.Region = dto.Region;
            entity.PostalCode = dto.PostalCode;
            entity.Country = dto.Country;
            entity.Phone = dto.Phone;
            entity.Fax = dto.Fax;
            entity.HomePage = dto.HomePage;
            entity.IsDeleted = dto.IsDeleted;

            await _db.SaveChangesAsync(ct);

            return await SupplierReadProjections.Build(
                    _db.Suppliers.AsNoTracking()
                        .Where(s => s.SupplierID == entity.SupplierID),
                    ProductListProjections.Build(
                        _db.Products.AsNoTracking()))
                        .FirstAsync(ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var affected = await _db.Suppliers
                .Where(c => c.SupplierID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(c => c.IsDeleted, true), ct);

            return affected > 0;
        }

        public async Task<bool> RestoreAsync(int id, CancellationToken ct)
        {
            var affected = await _db.Suppliers
                .Where(c => c.SupplierID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(c => c.IsDeleted, false), ct);
            return affected > 0;
        }

        private IQueryable<SupplierListDto> BuildSupplierListQuery()
        {
            return SupplierListProjections.Build(
                _db.Suppliers.AsNoTracking());
        }

        private IQueryable<SupplierReadDto> BuildSupplierReadQuery()
        {
            return SupplierReadProjections.Build(
                _db.Suppliers.AsNoTracking(),
                ProductListProjections.Build(
                    _db.Products.AsNoTracking()));
        }
    }
}
