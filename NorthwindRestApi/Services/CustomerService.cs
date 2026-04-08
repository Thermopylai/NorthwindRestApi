using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Common;
using NorthwindRestApi.Data;
using NorthwindRestApi.DTOs.Customers;
using NorthwindRestApi.Extensions;
using NorthwindRestApi.Models.Entities;
using NorthwindRestApi.Projections;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly NorthwindOriginalContext _db;

        public CustomerService(NorthwindOriginalContext db)
        {
            _db = db;
        }

        public async Task<List<CustomerListDto>> GetAllAsync(CancellationToken ct)
        {
            return await BuildCustomerListQuery()
                .OrderBy(c => c.CustomerID)
                .ToListAsync(ct);
        }

        public async Task<CustomerReadDto?> GetByIdAsync(string id, CancellationToken ct)
        {
            return await BuildCustomerReadQuery()
                .FirstOrDefaultAsync(c => c.CustomerID == id, ct);
        }

        public async Task<PagedResult<CustomerListDto>> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken ct)
        {
            return await BuildCustomerListQuery()
                .OrderBy(c => c.CustomerID)
                .ToPagedResultAsync(page, pageSize, ct);
        }

        public async Task<PagedResult<CustomerListDto>> SearchAsync(
            CustomerQueryParameters parameters,
            CancellationToken ct)
        {
            var query = BuildCustomerListQuery()
                .ApplyFilter(parameters)
                .ApplySearch(parameters.SearchTerm)
                .ApplySorting(parameters.OrderBy, parameters.Descending);

            return await query.ToPagedResultAsync(parameters.Page, parameters.PageSize, ct);
        }

        public async Task<CustomerReadDto> CreateAsync(CustomerCreateDto dto, CancellationToken ct)
        {
            var entity = new Customer
            {
                CustomerID = dto.CustomerID,
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
                IsDeleted = dto.IsDeleted
            };

            _db.Customers.Add(entity);
            await _db.SaveChangesAsync(ct);

            return await CustomerReadProjections.Build(
                    _db.Customers.AsNoTracking()
                        .Where(c => c.CustomerID == entity.CustomerID),
                    OrderReadProjections.Build(
                        _db.Orders.AsNoTracking()))
                        .FirstAsync(ct);
        }

        public async Task<CustomerReadDto?> UpdateAsync(string id, CustomerUpdateDto dto, CancellationToken ct)
        {
            var entity = await _db.Customers.FindAsync(new object[] { id }, ct);

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
            entity.IsDeleted = dto.IsDeleted;

            await _db.SaveChangesAsync(ct);

            return await CustomerReadProjections.Build(
                    _db.Customers.AsNoTracking()
                        .Where(c => c.CustomerID == entity.CustomerID),
                    OrderReadProjections.Build(
                        _db.Orders.AsNoTracking()))
                        .FirstAsync(ct);
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken ct)
        {
            var affected = await _db.Customers
                .Where(c => c.CustomerID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(c => c.IsDeleted, true), ct);

            return affected > 0;
        }
        
        public async Task<bool> RestoreAsync(string id, CancellationToken ct)
        {
            var affected = await _db.Customers
                .Where(c => c.CustomerID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(c => c.IsDeleted, false), ct);

            return affected > 0;
        }

        private IQueryable<CustomerListDto> BuildCustomerListQuery()
        {
            return CustomerListProjections.Build(_db.Customers.AsNoTracking());
        }

        private IQueryable<CustomerReadDto> BuildCustomerReadQuery()
        {
            return CustomerReadProjections.Build(
                _db.Customers.AsNoTracking(),
                OrderReadProjections.Build(
                    _db.Orders.AsNoTracking()));
        }
    }
}