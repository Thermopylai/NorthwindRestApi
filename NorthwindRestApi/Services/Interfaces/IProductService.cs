using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.DTOs.Products;

namespace NorthwindRestApi.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductListDto>> GetAllAsync(CancellationToken ct);
        Task<ProductReadDto?> GetByIdAsync(int id, CancellationToken ct);

        Task<List<ProductReadDto>> GetByCategoryIdAsync(int categoryId, CancellationToken ct);

        Task<PagedResult<ProductReadDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
        Task<PagedResult<ProductReadDto>> SearchAsync(ProductQueryParameters parameters, CancellationToken ct);

        Task<ProductReadDto> CreateAsync(ProductCreateDto dto, CancellationToken ct);
        Task<ProductReadDto?> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
        Task<bool> RestoreAsync(int id, CancellationToken ct);
    }
}
