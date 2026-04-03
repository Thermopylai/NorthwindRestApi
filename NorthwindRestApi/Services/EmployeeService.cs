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

        public async Task<PagedResult<EmployeeListDto>> GetPagedAsync(
            int page, 
            int pageSize, 
            CancellationToken ct)
        {
            return await BuildEmployeeListQuery()
                .OrderBy(e => e.EmployeeID)
                .ToPagedResultAsync(page, pageSize, ct);
        }

        public async Task<PagedResult<EmployeeListDto>> SearchAsync(
            EmployeeQueryParameters parameters, 
            CancellationToken ct)
        {
            var query = BuildEmployeeListQuery()
                .ApplyFilter(parameters)
                .ApplySearch(parameters.SearchTerm)
                .ApplySorting(parameters.OrderBy, parameters.Descending);

            return await query.ToPagedResultAsync(parameters.Page, parameters.PageSize, ct);
        }

        public async Task<EmployeeReadDto> CreateAsync(EmployeeCreateDto dto, CancellationToken ct)
        {
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
                PhotoPath = dto.PhotoPath,
                IsDeleted = dto.IsDeleted,
                Territories = dto.Territories.Select(et => new Territory
                {
                    TerritoryID = et.TerritoryID,
                    TerritoryDescription = et.TerritoryDescription,
                    RegionID = et.RegionID,
                })
                .ToList()
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

        public async Task<EmployeeReadDto?> UpdateAsync(int id, EmployeeUpdateDto dto, CancellationToken ct)
        {
            var entity = await _db.Employees.FindAsync(new object[] { id }, ct);

            if (entity == null)
                return null;

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
            entity.PhotoPath = dto.PhotoPath;
            entity.IsDeleted = dto.IsDeleted;
            entity.Territories = dto.Territories.Select(et => new Territory
            {
                TerritoryID = et.TerritoryID,
                TerritoryDescription = et.TerritoryDescription,
                RegionID = et.RegionID,
            }).ToList();
            
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
