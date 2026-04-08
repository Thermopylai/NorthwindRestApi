using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Customers;
using NorthwindRestApi.DTOs.Orders;

namespace NorthwindRestApi.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerListDto>> GetAllAsync(CancellationToken ct);
        Task<CustomerReadDto?> GetByIdAsync(string id, CancellationToken ct);
        Task<PagedResult<CustomerListDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
        Task<PagedResult<CustomerListDto>> SearchAsync(CustomerQueryParameters parameters, CancellationToken ct);
        Task<CustomerReadDto> CreateAsync(CustomerCreateDto dto, CancellationToken ct);
        Task<CustomerReadDto?> UpdateAsync(string id, CustomerUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(string id, CancellationToken ct);
        Task<bool> RestoreAsync(string id, CancellationToken ct);
    }
}
