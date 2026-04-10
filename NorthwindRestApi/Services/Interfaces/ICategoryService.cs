using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Categories;

namespace NorthwindRestApi.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryListDto>> GetAllAsync(CancellationToken ct);
        Task<CategoryReadDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<PagedResult<CategoryReadDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
        Task<PagedResult<CategoryReadDto>> SearchAsync(CategoryQueryParameters parameters, CancellationToken ct);
        Task<CategoryReadDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct);
        Task<CategoryReadDto?> UpdateAsync(int id, CategoryUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
        Task<bool> RestoreAsync(int id, CancellationToken ct);
    }
}
