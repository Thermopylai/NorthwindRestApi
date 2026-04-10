using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Suppliers;

namespace NorthwindRestApi.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<List<SupplierListDto>> GetAllAsync(CancellationToken ct);
        Task<SupplierReadDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<PagedResult<SupplierReadDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
        Task<PagedResult<SupplierReadDto>> SearchAsync(SupplierQueryParameters parameters, CancellationToken ct);
        Task<SupplierReadDto> CreateAsync(SupplierCreateDto dto, CancellationToken ct);
        Task<SupplierReadDto?> UpdateAsync(int id, SupplierUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
        Task<bool> RestoreAsync(int id, CancellationToken ct);
    }
}
