using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Common;
using NorthwindRestApi.Data;
using NorthwindRestApi.DTOs.Employees;
using NorthwindRestApi.DTOs.Order_Details;
using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.DTOs.Territories;
using NorthwindRestApi.Extensions;
using NorthwindRestApi.Models;
using NorthwindRestApi.Models.Entities;
using NorthwindRestApi.Projections;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly NorthwindOriginalContext _db;

        public EmployeeService(NorthwindOriginalContext db)
        {
            _db = db;
        }

        public async Task<List<EmployeeListDto>> GetAllAsync(CancellationToken ct)
        {
            return await BuildEmployeeListQuery()
                .OrderBy(e => e.EmployeeID)
                .ToListAsync(ct);
        }

        public async Task<EmployeeReadDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await BuildEmployeeReadQuery()
                .FirstOrDefaultAsync(e => e.EmployeeID == id, ct);
        }

        public async Task<PagedResult<EmployeeReadDto>> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken ct)
        {
            return await BuildEmployeeReadQuery()
                .OrderBy(e => e.EmployeeID)
                .ToPagedResultAsync(page, pageSize, ct);
        }

        public async Task<PagedResult<EmployeeReadDto>> SearchAsync(
            EmployeeQueryParameters parameters,
            CancellationToken ct)
        {
            var query = BuildEmployeeReadQuery()
                .ApplyFilter(parameters)
                .ApplySearch(parameters.SearchTerm)
                .ApplySorting(parameters.OrderBy, parameters.Descending);

            return await query.ToPagedResultAsync(parameters.Page, parameters.PageSize, ct);
        }

        public async Task<EmployeeReadDto> CreateAsync([FromForm] EmployeeCreateDto dto, CancellationToken ct)
        {
            byte[]? imageBytes = null;

            if (dto.Photo != null && dto.Photo.Length > 0)
            {
                using var ms = new MemoryStream();
                await dto.Photo.CopyToAsync(ms, ct);
                imageBytes = ms.ToArray();
                imageBytes = ImageConverter.AddNorthwindPictureHeader(imageBytes);
            }

            var territories = await ResolveTerritoriesAsync(dto.Territories, ct);

            var entity = new Employee
            {
                LastName = dto.LastName,
                FirstName = dto.FirstName,
                Title = dto.Title,
                TitleOfCourtesy = dto.TitleOfCourtesy,
                BirthDate = dto.BirthDate,
                HireDate = dto.HireDate,
                Address = dto.Address,
                City = dto.City,
                Region = dto.Region,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                HomePhone = dto.HomePhone,
                Extension = dto.Extension,
                Notes = dto.Notes,
                ReportsTo = dto.ReportsTo,
                Photo = imageBytes,
                PhotoPath = dto.PhotoPath,
                IsDeleted = dto.IsDeleted,
                Territories = territories
            };

            _db.Add(entity);
            await _db.SaveChangesAsync(ct);

            return await EmployeeReadProjections.Build(
                    _db.Employees.AsNoTracking()
                        .Where(c => c.EmployeeID == entity.EmployeeID),
                    OrderReadProjections.Build(
                        _db.Orders.AsNoTracking()))
                        .FirstAsync(ct);
        }

        public async Task<EmployeeReadDto?> UpdateAsync(int id, [FromForm] EmployeeUpdateDto dto, CancellationToken ct)
        {
            var entity = await _db.Employees.FindAsync(new object[] { id }, ct);

            if (entity == null)
                return null;

            byte[]? imageBytes = null;

            if (dto.Photo != null && dto.Photo.Length > 0)
            {
                using var ms = new MemoryStream();
                await dto.Photo.CopyToAsync(ms, ct);
                imageBytes = ms.ToArray();
                imageBytes = ImageConverter.AddNorthwindPictureHeader(imageBytes);
            }

            var territories = await ResolveTerritoriesAsync(dto.Territories, ct);

            entity.LastName = dto.LastName;
            entity.FirstName = dto.FirstName;
            entity.Title = dto.Title;
            entity.TitleOfCourtesy = dto.TitleOfCourtesy;
            entity.BirthDate = dto.BirthDate;
            entity.HireDate = dto.HireDate;
            entity.Address = dto.Address;
            entity.City = dto.City;
            entity.Region = dto.Region;
            entity.PostalCode = dto.PostalCode;
            entity.Country = dto.Country;
            entity.HomePhone = dto.HomePhone;
            entity.Extension = dto.Extension;
            entity.Notes = dto.Notes;
            entity.ReportsTo = dto.ReportsTo;
            entity.Photo = imageBytes ?? entity.Photo;
            entity.PhotoPath = dto.PhotoPath;
            entity.IsDeleted = dto.IsDeleted;

            // Update many-to-many via tracked entities (updates join table, doesn't insert Territories)
            await _db.Entry(entity).Collection(e => e.Territories).LoadAsync(ct);
            entity.Territories.Clear();
            foreach (var territory in territories)
            {
                entity.Territories.Add(territory);
            }

            await _db.SaveChangesAsync(ct);

            return await EmployeeReadProjections.Build(
                    _db.Employees.AsNoTracking()
                        .Where(c => c.EmployeeID == entity.EmployeeID),
                    OrderReadProjections.Build(
                        _db.Orders.AsNoTracking()))
                        .FirstAsync(ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var affected = await _db.Employees
                .Where(c => c.EmployeeID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(c => c.IsDeleted, true), ct);

            return affected > 0;
        }

        public async Task<bool> RestoreAsync(int id, CancellationToken ct)
        {
            var affected = await _db.Employees
                .Where(c => c.EmployeeID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(c => c.IsDeleted, false), ct);
            return affected > 0;
        }

        private async Task<List<Territory>> ResolveTerritoriesAsync(IEnumerable<string> territoryIds, CancellationToken ct)
        {
            var ids = territoryIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (ids.Count == 0)
                return new List<Territory>();

            // IMPORTANT: do NOT use AsNoTracking here; we want tracked entities for relationship fixup.
            return await _db.Territories
                .Where(t => ids.Contains(t.TerritoryID))
                .ToListAsync(ct);
        }

        private IQueryable<EmployeeListDto> BuildEmployeeListQuery()
        {
            return EmployeeListProjections.Build(_db.Employees.AsNoTracking());
        }

        private IQueryable<EmployeeReadDto> BuildEmployeeReadQuery()
        {
            return EmployeeReadProjections.Build(
                _db.Employees.AsNoTracking(),
                OrderReadProjections.Build(
                    _db.Orders.AsNoTracking()));
        }
    }
}
