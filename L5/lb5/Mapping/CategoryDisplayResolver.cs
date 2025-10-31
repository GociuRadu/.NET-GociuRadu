using AutoMapper;
using Labb5.DTOs;

namespace Labb5.Mapping;

public class CategoryDisplayResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product src, ProductProfileDto dest, string destMember, ResolutionContext context)
        => src.Category switch
        {
            ProductCategory.Electronics => "Electronics & Technology",
            ProductCategory.Clothing    => "Clothing & Fashion",
            ProductCategory.Books       => "Books & Media",
            ProductCategory.Home        => "Home & Garden",
            _                           => "Uncategorized"
        };
}