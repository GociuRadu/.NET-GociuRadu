using AutoMapper;
using Labb5.DTOs;
namespace Labb5.Mapping;

public class AdvancedProductMappingProfile : Profile{
    
public AdvancedProductMappingProfile()
{
    CreateMap<CreateProductProfileRequest, Product>()
        .ForMember(dest => dest.Id,
            opt => opt.MapFrom(src => Guid.NewGuid()))
        .ForMember(dest => dest.CreatedAt,
            opt => opt.MapFrom(src => DateTime.UtcNow))
        .ForMember(dest => dest.IsAvailable,
            opt => opt.MapFrom(src => src.StockQuantity > 0))
        .ForMember(dest => dest.ImageUrl,
            opt => opt.MapFrom((src, _) =>
                src.Category == ProductCategory.Home ? null : src.ImageUrl))
        .ForMember(dest => dest.Price,
            opt => opt.MapFrom(src =>
                src.Category == ProductCategory.Home ? src.Price * 0.9m : src.Price))
        .ForMember(dest => dest.Category,
            opt => opt.MapFrom(src => src.Category));
    //
    // 2️⃣ Product → ProductProfileDto
    //
    CreateMap<Product, ProductProfileDto>()
        .ForMember(dest => dest.CategoryDisplayName,
            opt => opt.MapFrom<CategoryDisplayResolver>())
        .ForMember(dest => dest.FormattedPrice,
            opt => opt.MapFrom<PriceFormatterResolver>())
        .ForMember(dest => dest.ProductAge,
            opt => opt.MapFrom<ProductAgeResolver>())
        .ForMember(dest => dest.BrandInitials,
            opt => opt.MapFrom<BrandInitialsResolver>())
        .ForMember(dest => dest.AvailabilityStatus,
            opt => opt.MapFrom<AvailabilityStatusResolver>());
}
}
