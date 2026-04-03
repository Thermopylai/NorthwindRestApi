namespace NorthwindRestApi.DTOs.Employees
{
    public class EmployeeQueryParameters
    {
        // Filter parameters
        public bool? IsDeleted { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public int? ReportsTo { get; set; }

        // Search parameter
        public string? SearchTerm { get; set; }

        // Sorting parameters
        public string? OrderBy { get; set; }
        public bool Descending { get; set; } = false;

        // Pagination parameters
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
