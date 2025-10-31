namespace Labb5.DTOs;

public class ProductProfileDto
{
    // Core identifiers & basic info
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;

    // Display-only, derived from enum category (via CategoryDisplayResolver)
    public string CategoryDisplayName { get; set; } = string.Empty;

    // Pricing
    public decimal Price { get; set; }                  // raw numeric price
    public string FormattedPrice { get; set; } = "";    

    // Dates
    public DateTime ReleaseDate { get; set; }
    public DateTime CreatedAt { get; set; }

    // Media
    public string? ImageUrl { get; set; }

    // Availability / stock
    public bool IsAvailable { get; set; }
    public int StockQuantity { get; set; }

    // Enriched, human-friendly fields (computed in mapping profile)
    public string ProductAge { get; set; } = "";       
    public string BrandInitials { get; set; } = "";    
    public string AvailabilityStatus { get; set; } = "";
}
