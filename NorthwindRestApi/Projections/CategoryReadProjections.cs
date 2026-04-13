using NorthwindRestApi.Common;
using NorthwindRestApi.DTOs.Categories;
using NorthwindRestApi.DTOs.Products;
using NorthwindRestApi.Models.Entities;

namespace NorthwindRestApi.Projections
{
    public static class CategoryReadProjections
    {
        public static IQueryable<CategoryReadDto> Build(
            IQueryable<Category> categories,
            IQueryable<ProductListDto> products)
        {
            return categories
                .Select(c => new CategoryReadDto
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName,
                    VatRate = VatRules.ReducedVatCategories.Contains(c.CategoryName)
                                    ? VatRules.ReducedVatRate
                                    : VatRules.StandardVatRate,
                    Description = c.Description,
                    IsDeleted = c.IsDeleted,

                    Products = products
                        .Where(p => p.CategoryID == c.CategoryID)
                        .OrderBy(p => p.ProductName)
                        .ToList(),
                    ProductCount = products
                        .Count(p => p.CategoryID == c.CategoryID),
                    Picture = c.Picture != null ? ImageConverter.ConvertToBase64(c.Picture) : null
                });
        }
    }
}
