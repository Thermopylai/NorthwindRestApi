using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Orders;

namespace NorthwindRestApi.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderListDto>> GetAllAsync(CancellationToken ct);
        Task<OrderReadDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<List<OrderReadDto>> GetByCustomerIdAsync(string customerId, CancellationToken ct);
        Task<List<OrderReadDto>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken ct);
        Task<PagedResult<OrderReadDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
        Task<PagedResult<OrderReadDto>> SearchAsync(OrderQueryParameters parameters, CancellationToken ct);
        Task<OrderReadDto> CreateAsync(OrderCreateDto dto, CancellationToken ct);
        Task<OrderReadDto?> UpdateAsync(int id, OrderUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
        Task<bool> RestoreAsync(int id, CancellationToken ct);
    }
}
