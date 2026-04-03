using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Orders;

namespace NorthwindRestApi.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderListDto>> GetAllAsync(CancellationToken ct);
        Task<OrderReadDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<List<OrderListDto>> GetByCustomerIdAsync(string customerId, CancellationToken ct);
        Task<List<OrderListDto>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken ct);
        Task<PagedResult<OrderListDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
        Task<PagedResult<OrderListDto>> SearchAsync(OrderQueryParameters parameters, CancellationToken ct);
        Task<OrderReadDto> CreateAsync(OrderCreateDto dto, CancellationToken ct);
        Task<OrderReadDto?> UpdateAsync(int id, OrderUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
