using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Employees;
using NorthwindRestApi.DTOs.Orders;

namespace NorthwindRestApi.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<EmployeeListDto>> GetAllAsync(CancellationToken ct);
        Task<EmployeeReadDto?> GetByIdAsync(int id, CancellationToken ct);
        Task<PagedResult<EmployeeReadDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
        Task<PagedResult<EmployeeReadDto>> SearchAsync(EmployeeQueryParameters parameters, CancellationToken ct);
        Task<EmployeeReadDto> CreateAsync(EmployeeCreateDto dto, CancellationToken ct);
        Task<EmployeeReadDto?> UpdateAsync(int id, EmployeeUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
        Task<bool> RestoreAsync(int id, CancellationToken ct);
    }
}
