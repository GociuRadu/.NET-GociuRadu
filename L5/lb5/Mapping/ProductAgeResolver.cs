using AutoMapper;
using Labb5.DTOs;

namespace Labb5.Mapping;

public class ProductAgeResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product src, ProductProfileDto dest, string destMember, ResolutionContext context)
    {
        var days = (DateTime.UtcNow - src.ReleaseDate).TotalDays;
        if (days < 30) return "New Release";
        if (days < 365) return $"{Math.Floor(days / 30)} months old";
        if (days < 1825) return $"{Math.Floor(days / 365)} years old";
        return "Classic";
    }
}