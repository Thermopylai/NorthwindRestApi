using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Order_Details;
using NorthwindRestApi.DTOs.Orders;

namespace NorthwindRestApi.Services.Interfaces
{
    public interface IOrder_DetailService
    {
        Task<List<Order_DetailReadDto>> GetAllAsync(CancellationToken ct);
        Task<Order_DetailReadDto?> GetByIdAsync(int orderId, int productId, CancellationToken ct);

        Task<List<Order_DetailReadDto>> GetByOrderIdAsync(int orderId, CancellationToken ct);
        Task<List<Order_DetailReadDto>> GetByProductIdAsync(int productId, CancellationToken ct);

        Task<PagedResult<Order_DetailReadDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
        Task<PagedResult<Order_DetailReadDto>> SearchAsync(Order_DetailQueryParameters parameters, CancellationToken ct);
        Task<Order_DetailReadDto> CreateAsync(Order_DetailCreateDto dto, CancellationToken ct);
        Task<Order_DetailReadDto?> UpdateAsync(int orderId, int productId,Order_DetailUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(int orderId, int productId, CancellationToken ct);
    }
}
