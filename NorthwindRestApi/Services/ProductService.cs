using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Common;
using NorthwindRestApi.Data;
using NorthwindRestApi.DTOs.Products;
using NorthwindRestApi.Extensions;
using NorthwindRestApi.Models;
using NorthwindRestApi.Models.Entities;
using NorthwindRestApi.Projections;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Services
{
    public class ProductService : IProductService
    {
        private readonly NorthwindOriginalContext _db;
        
        public ProductService(NorthwindOriginalContext db)
        {
            _db = db;
        }

        public async Task<List<ProductListDto>> GetAllAsync(CancellationToken ct)
        {
            return await BuildProductListQuery()
                .OrderBy(p => p.ProductID)
                .ToListAsync(ct);
        }

        public async Task<ProductReadDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await BuildProductReadQuery()
                .FirstOrDefaultAsync(p => p.ProductID == id, ct);
        }

        public async Task<List<ProductReadDto>> GetByCategoryIdAsync(int categoryId, CancellationToken ct)
        {
            return await BuildProductReadQuery()
                .Where(p => p.CategoryID == categoryId)
                .OrderBy(p => p.ProductID)
                .ToListAsync(ct);
        }

        public async Task<PagedResult<ProductReadDto>> GetPagedAsync(
            int page, 
            int pageSize, 
            CancellationToken ct)
        {           
            return await BuildProductReadQuery()
                .OrderBy(p => p.ProductID)
                .ToPagedResultAsync(page, pageSize, ct);
        }

        public async Task<PagedResult<ProductReadDto>> SearchAsync(
            ProductQueryParameters parameters, 
            CancellationToken ct)
        {
            var query = BuildProductReadQuery()
                .ApplyFilter(parameters)
                .ApplySearch(parameters.SearchTerm)
                .ApplySorting(parameters.OrderBy, parameters.Descending);

            return await query.ToPagedResultAsync(parameters.Page, parameters.PageSize, ct);
        }

        public async Task<ProductReadDto> CreateAsync(ProductCreateDto dto, CancellationToken ct)
        {
            var entity = new Product
            {
                ProductName = dto.ProductName,
                SupplierID = dto.SupplierID,
                CategoryID = dto.CategoryID,
                QuantityPerUnit = dto.QuantityPerUnit,
                UnitPrice = dto.UnitPrice,
                UnitsInStock = dto.UnitsInStock,
                UnitsOnOrder = dto.UnitsOnOrder,
                ReorderLevel = dto.ReorderLevel,
                Discontinued = dto.Discontinued,
                ImageLink = dto.ImageLink
            };

            _db.Products.Add(entity);
            await _db.SaveChangesAsync(ct);

            await _db.Entry(entity)
                .Reference(p => p.Category)
                .LoadAsync(ct);

            return await ProductReadProjections.Build(
                _db.Products.AsNoTracking()
                    .Where(p => p.ProductID == entity.ProductID),
                Order_DetailReadProjections.Build(
                    _db.Order_Details.AsNoTracking()),
                CustomerListProjections.Build(
                    _db.Customers.AsNoTracking()))
                    .FirstAsync(ct);
        }

        public async Task<ProductReadDto?> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken ct)
        {
            var entity = await _db.Products.FindAsync(new object[] { id }, ct);

            if (entity == null)
                return null;

            entity.ProductName = dto.ProductName;
            entity.SupplierID = dto.SupplierID;
            entity.CategoryID = dto.CategoryID;
            entity.QuantityPerUnit = dto.QuantityPerUnit;
            entity.UnitPrice = dto.UnitPrice;
            entity.UnitsInStock = dto.UnitsInStock;
            entity.UnitsOnOrder = dto.UnitsOnOrder;
            entity.ReorderLevel = dto.ReorderLevel;
            entity.Discontinued = dto.Discontinued;
            entity.ImageLink = dto.ImageLink;

            await _db.SaveChangesAsync(ct);

            return await ProductReadProjections.Build(
                _db.Products.AsNoTracking()
                    .Where(p => p.ProductID == entity.ProductID),
                Order_DetailReadProjections.Build(
                    _db.Order_Details.AsNoTracking()),
                CustomerListProjections.Build(
                    _db.Customers.AsNoTracking()))
                    .FirstAsync(ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var affected = await _db.Products
                .Where(p => p.ProductID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(p => p.Discontinued, true), ct);

            return affected > 0;
        }

        public async Task<bool> RestoreAsync(int id, CancellationToken ct)
        {
            var affected = await _db.Products
                .Where(p => p.ProductID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(p => p.Discontinued, false), ct);
            return affected > 0;
        }

        private IQueryable<ProductListDto> BuildProductListQuery()
        {
            return ProductListProjections.Build(
                _db.Products.AsNoTracking());
        }

        private IQueryable<ProductReadDto> BuildProductReadQuery()
        {
            return ProductReadProjections.Build(
                _db.Products.AsNoTracking(),
                Order_DetailReadProjections.Build(
                    _db.Order_Details.AsNoTracking()),
                CustomerListProjections.Build(
                    _db.Customers.AsNoTracking()));
        }
    }
}
