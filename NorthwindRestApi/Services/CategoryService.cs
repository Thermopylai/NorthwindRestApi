using Microsoft.AspNetCore.Mvc;
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

        public async Task<List<CategoryReadDto>> GetAllAsync(CancellationToken ct)
        {
            return await BuildCategoryReadQuery()
                .OrderBy(o => o.CategoryID)
                .ToListAsync(ct);
        }

        public async Task<CategoryReadDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            return await BuildCategoryReadQuery()
                .FirstOrDefaultAsync(c => c.CategoryID == id, ct);
        }

        public async Task<PagedResult<CategoryReadDto>> GetPagedAsync(
            int page, 
            int pageSize, 
            CancellationToken ct)
        {
            return await BuildCategoryReadQuery()
                .OrderBy(o => o.CategoryID)
                .ToPagedResultAsync(page, pageSize, ct);
        }

        public async Task<PagedResult<CategoryReadDto>> SearchAsync(
            CategoryQueryParameters parameters, 
            CancellationToken ct)
        {
            var query = BuildCategoryReadQuery()
                .ApplyFilter(parameters)
                .ApplySearch(parameters.SearchTerm)
                .ApplySorting(parameters.OrderBy, parameters.Descending);

            return await query.ToPagedResultAsync(parameters.Page, parameters.PageSize, ct);
        }

        public async Task<CategoryReadDto> CreateAsync(
            [FromForm]
            CategoryCreateDto dto, 
            CancellationToken ct)
        {
            byte[]? imageBytes = null;

            if (dto.Picture != null && dto.Picture.Length > 0)
            {
                using var ms = new MemoryStream();
                await dto.Picture.CopyToAsync(ms, ct);
                imageBytes = ms.ToArray();
                imageBytes = ImageConverter.AddNorthwindPictureHeader(imageBytes);
            }
            
            var entity = new Category
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description,
                Picture = imageBytes,
                IsDeleted = dto.IsDeleted
            };

            _db.Categories.Add(entity);
            await _db.SaveChangesAsync(ct);
            
            return await CategoryReadProjections.Build(
                    _db.Categories.AsNoTracking()
                        .Where(c => c.CategoryID == entity.CategoryID),
                    ProductListProjections.Build(
                        _db.Products.AsNoTracking()))
                        .FirstAsync(ct);
        }

        public async Task<CategoryReadDto?> UpdateAsync(
            int id,
            [FromForm]
            CategoryUpdateDto dto, 
            CancellationToken ct)
        {
            var entity = await _db.Categories.FindAsync(new object[] { id }, ct);

            if (entity == null)
                return null;

            byte[]? imageBytes = null;

            if (dto.Picture != null && dto.Picture.Length > 0)
            {
                using var ms = new MemoryStream();
                await dto.Picture.CopyToAsync(ms, ct);
                imageBytes = ms.ToArray();
                imageBytes = ImageConverter.AddNorthwindPictureHeader(imageBytes);
            }

            entity.CategoryName = dto.CategoryName;
            entity.Description = dto.Description;
            entity.Picture = imageBytes ?? entity.Picture;
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
