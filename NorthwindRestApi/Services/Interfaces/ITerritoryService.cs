using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Territories;

namespace NorthwindRestApi.Services.Interfaces
{
    public interface ITerritoryService
    {
        Task<List<TerritoryReadDto>> GetAllAsync(CancellationToken ct);
        Task<TerritoryReadDto?> GetByIdAsync(string id, CancellationToken ct);
        Task<PagedResult<TerritoryReadDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
        Task<PagedResult<TerritoryReadDto>> SearchAsync(TerritoryQueryParameters parameters, CancellationToken ct);
        Task<TerritoryReadDto> CreateAsync(TerritoryCreateDto dto, CancellationToken ct);
        Task<TerritoryReadDto?> UpdateAsync(string id, TerritoryUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(string id, CancellationToken ct);
    }
}
