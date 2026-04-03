using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.DTOs.Territories;

namespace NorthwindRestApi.DTOs.Employees
{
    public class EmployeeListDto
    {
        public int EmployeeID { get; set; }
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? Title { get; set; }
        public string? TitleOfCourtesy { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? HomePhone { get; set; }
        public string? Extension { get; set; }
        public string? Notes { get; set; }
        public int? ReportsTo { get; set; }
        public string? ReportsToFullName { get; set; }
        public string? PhotoPath { get; set; }
        public bool IsDeleted { get; set; }
               
        public List<TerritoryReadDto> Territories { get; set; } = new();
        public int? TerritoryCount { get; set; }
    }
}
