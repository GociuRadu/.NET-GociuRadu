using AutoMapper;
using Labb5.DTOs;

namespace Labb5.Mapping;

public class AvailabilityStatusResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product src, ProductProfileDto dest, string destMember, ResolutionContext context)
    {
        if (!src.IsAvailable) return "Out of Stock";
        if (src.StockQuantity <= 0) return "Unavailable";
        if (src.StockQuantity == 1) return "Last Item";
        if (src.StockQuantity <= 5) return "Limited Stock";
        return "In Stock";
    }
}