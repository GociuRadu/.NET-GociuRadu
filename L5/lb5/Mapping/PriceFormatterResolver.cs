using AutoMapper;
using System.Globalization;
using Labb5.DTOs;

namespace Labb5.Mapping;

public class PriceFormatterResolver : IValueResolver<Product, ProductProfileDto, string>
{
    public string Resolve(Product src, ProductProfileDto dest, string destMember, ResolutionContext context)
        => src.Price.ToString("C2", CultureInfo.CurrentCulture);
}