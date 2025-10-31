namespace Labb5.DTOs;

public class CreateProductProfileRequest
{
    // Basic info
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;

    // Category
    public ProductCategory Category { get; set; }

    // Pricing & release
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }

    // Optional fields
    public string? ImageUrl { get; set; }

    // Stock info
    public int StockQuantity { get; set; } = 1;
}
