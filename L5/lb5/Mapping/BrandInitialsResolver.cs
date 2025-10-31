using AutoMapper;
using Labb5.DTOs;

namespace Labb5.Mapping;

public class BrandInitialsResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product src, ProductProfileDto dest, string destMember, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(src.Brand))
            return "?";

        var parts = src.Brand.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[^1][0])}"
            : $"{char.ToUpper(src.Brand[0])}";
    }
}