using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Shippers;

namespace NorthwindRestApi.Services.Interfaces
{
    public interface IShipperService
    {
        Task<List<ShipperReadDto>> GetAllAsync(CancellationToken ct);
        Task<ShipperReadDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<PagedResult<ShipperReadDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
        Task<PagedResult<ShipperReadDto>> SearchAsync(ShipperQueryParameters parameters, CancellationToken ct);
        Task<ShipperReadDto> CreateAsync(ShipperCreateDto dto, CancellationToken ct);
        Task<ShipperReadDto?> UpdateAsync(int id, ShipperUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
        Task<bool> RestoreAsync(int id, CancellationToken ct);
    }
}
