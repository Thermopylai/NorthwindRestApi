using Microsoft.EntityFrameworkCore;
using NorthwindRestApi.Common;
using NorthwindRestApi.Data;
using NorthwindRestApi.DTOs.Categories;
using NorthwindRestApi.DTOs.Products;
using NorthwindRestApi.Extensions;
using NorthwindRestApi.Models;
using NorthwindRestApi.Models.Entities;
using NorthwindRestApi.Projections;
using NorthwindRestApi.Services.Interfaces;

namespace NorthwindRestApi.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly NorthwindOriginalContext _db;

        public CategoryService(NorthwindOriginalContext db)
        {
            _db = db;
        }

        public async Task<List<CategoryListDto>> GetAllAsync(CancellationToken ct)
        {
            return await BuildCategoryListQuery()
                .OrderBy(o => o.CategoryID)
                .ToListAsync(ct);
        }

        public async Task<CategoryReadDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await BuildCategoryReadQuery()
                .FirstOrDefaultAsync(c => c.CategoryID == id, ct);
        }

        public async Task<PagedResult<CategoryListDto>> GetPagedAsync(
            int page, 
            int pageSize, 
            CancellationToken ct)
        {
            return await BuildCategoryListQuery()
                .OrderBy(o => o.CategoryID)
                .ToPagedResultAsync(page, pageSize, ct);
        }

        public async Task<PagedResult<CategoryListDto>> SearchAsync(
            CategoryQueryParameters parameters, 
            CancellationToken ct)
        {
            var query = BuildCategoryListQuery()
                .ApplyFilter(parameters)
                .ApplySearch(parameters.SearchTerm)
                .ApplySorting(parameters.OrderBy, parameters.Descending);

            return await query.ToPagedResultAsync(parameters.Page, parameters.PageSize, ct);
        }

        public async Task<CategoryReadDto> CreateAsync(
            CategoryCreateDto dto, 
            CancellationToken ct)
        {
            var entity = new Category
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description,
                IsDeleted = dto.IsDeleted
            };

            _db.Categories.Add(entity);
            await _db.SaveChangesAsync(ct);

            await _db.Entry(entity)
                .Reference(c => c.Products)
                .LoadAsync(ct);

            return await CategoryReadProjections.Build(
                    _db.Categories.AsNoTracking()
                        .Where(c => c.CategoryID == entity.CategoryID),
                    ProductListProjections.Build(
                        _db.Products.AsNoTracking()))
                        .FirstAsync(ct);
        }

        public async Task<CategoryReadDto?> UpdateAsync(
            int id, 
            CategoryUpdateDto dto, 
            CancellationToken ct)
        {
            var entity = await _db.Categories.FindAsync(new object[] { id }, ct);

            if (entity == null)
                return null;

            entity.CategoryName = dto.CategoryName;
            entity.Description = dto.Description;
            entity.IsDeleted = dto.IsDeleted;

            await _db.SaveChangesAsync(ct);

            return await CategoryReadProjections.Build(
                    _db.Categories.AsNoTracking()
                        .Where(c => c.CategoryID == entity.CategoryID),
                    ProductListProjections.Build(
                        _db.Products.AsNoTracking()))
                        .FirstAsync(ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var affected = await _db.Categories
                .Where(c => c.CategoryID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(c => c.IsDeleted, true), ct);

            return affected > 0;
        }

        public async Task<bool> RestoreAsync(int id, CancellationToken ct)
        {
            var affected = await _db.Categories
                .Where(c => c.CategoryID == id)
                .ExecuteUpdateAsync(u => u.SetProperty(c => c.IsDeleted, false), ct);

            return affected > 0;
        }

        private IQueryable<CategoryListDto> BuildCategoryListQuery()
        {
            return CategoryListProjections.Build(
                _db.Categories.AsNoTracking());
        }

        private IQueryable<CategoryReadDto> BuildCategoryReadQuery()
        {
            return CategoryReadProjections.Build(
                _db.Categories.AsNoTracking(),
                ProductListProjections.Build(
                    _db.Products.AsNoTracking()));
        }
    }
}
