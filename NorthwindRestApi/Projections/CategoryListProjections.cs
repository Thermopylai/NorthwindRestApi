using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Categories;
using NorthwindRestApi.DTOs.Products;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class CategoryListProjections
    {
        public static IQueryable<CategoryListDto> Build(
            IQueryable<Category> categories)
        {
            return categories
                .Select(c => new CategoryListDto
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName,
                    VatRate = VatRules.ReducedVatCategories.Contains(c.CategoryName)
                                    ? VatRules.ReducedVatRate
                                    : VatRules.StandardVatRate,
                    Description = c.Description,
                    IsDeleted = c.IsDeleted
                });
        }
    }
}
