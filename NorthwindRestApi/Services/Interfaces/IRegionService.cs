using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Regions;

namespace NorthwindRestApi.Services.Interfaces
{
    public interface IRegionService
    {
        Task<List<RegionListDto>> GetAllAsync(CancellationToken ct);
        Task<RegionReadDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<PagedResult<RegionListDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
        Task<PagedResult<RegionListDto>> SearchAsync(RegionQueryParameters parameters, CancellationToken ct);
        Task<RegionReadDto> CreateAsync(RegionCreateDto dto, CancellationToken ct);
        Task<RegionReadDto?> UpdateAsync(int id, RegionUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
