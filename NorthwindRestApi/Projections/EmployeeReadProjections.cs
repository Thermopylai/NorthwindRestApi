using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Employees;
using NorthwindRestApi.DTOs.Orders;
using NorthwindRestApi.DTOs.Territories;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class EmployeeReadProjections
    {
        public static IQueryable<EmployeeReadDto> Build(
            IQueryable<Employee> employees,
            IQueryable<OrderReadDto> orders)
        {
            return employees
                .Select(e => new EmployeeReadDto
                {
                    EmployeeID = e.EmployeeID,
                    LastName = e.LastName,
                    FirstName = e.FirstName,
                    Title = e.Title,
                    TitleOfCourtesy = e.TitleOfCourtesy,
                    BirthDate = e.BirthDate,
                    HireDate = e.HireDate,
                    Address = e.Address,
                    City = e.City,
                    Region = e.Region,
                    PostalCode = e.PostalCode,
                    Country = e.Country,
                    HomePhone = e.HomePhone,
                    Extension = e.Extension,
                    Notes = e.Notes,
                    ReportsTo = e.ReportsTo,
                    ReportsToFullName = e.ReportsToNavigation != null
                        ? e.ReportsToNavigation.FirstName + " " + e.ReportsToNavigation.LastName
                        : null,
                    Photo = e.Photo != null ? ImageConverter.ConvertToBase64(e.Photo) : null,
                    PhotoPath = e.PhotoPath,
                    IsDeleted = e.IsDeleted,
                    Territories = e.Territories.Select(et => new TerritoryReadDto
                    {
                        TerritoryID = et.TerritoryID,
                        TerritoryDescription = et.TerritoryDescription,
                        RegionID = et.RegionID,
                        RegionDescription = et.Region.RegionDescription
                    })
                    .OrderBy(t => t.TerritoryID)
                    .ToList(),
                    TerritoryCount = e.Territories.Count,

                    Orders = orders
                        .Where(o => o.EmployeeID == e.EmployeeID)
                        .OrderByDescending(o => o.OrderDate)
                        .ToList(),
                    OrderCount = orders
                        .Count(o => o.EmployeeID == e.EmployeeID)
                });
        }
    }
}
