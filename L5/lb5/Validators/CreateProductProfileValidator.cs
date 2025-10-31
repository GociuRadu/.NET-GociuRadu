using FluentValidation;
using Labb5.DTOs;
using Labb5.Products;
using Microsoft.Extensions.Logging;

namespace Labb5.Validators;

/// <summary>
/// Complex validator for product creation with asynchronous checks and business rules.
/// Covers name, brand, SKU, price, dates, image URLs, and category-based conditions.
/// </summary>
public class CreateProductProfileValidator : AbstractValidator<CreateProductProfileRequest>
{
    private readonly ILogger<CreateProductProfileValidator> _logger;
    private static readonly List<Product> _existingProducts = new(); // Simulated DB

    public CreateProductProfileValidator(ILogger<CreateProductProfileValidator> logger)
    {
        _logger = logger;

        // -------- NAME --------
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .Length(1, 200)
            .Must(BeValidName).WithMessage("Product name contains inappropriate content.")
            .MustAsync(BeUniqueName).WithMessage("A product with the same name and brand already exists.");

        // -------- BRAND --------
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .Length(2, 100)
            .Matches(@"^[\p{L}\p{N}\s\-\.\']+$")
            .WithMessage("Brand name contains invalid characters.");

        // -------- SKU --------
        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .Matches(@"^[A-Za-z0-9\-]{5,20}$").WithMessage("Invalid SKU format (must be 5–20 chars, alphanumeric, hyphens allowed).")
            .MustAsync(BeUniqueSKU).WithMessage("SKU already exists.");

        // -------- CATEGORY --------
        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid product category.");

        // -------- PRICE --------
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.")
            .LessThan(10000).WithMessage("Price must be less than 10,000.");

        // -------- RELEASE DATE --------
        RuleFor(x => x.ReleaseDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Release date cannot be in the future.")
            .GreaterThan(new DateTime(1900, 1, 1)).WithMessage("Release date cannot be before 1900.");

        // -------- STOCK QUANTITY --------
        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.")
            .LessThanOrEqualTo(100000).WithMessage("Stock quantity exceeds allowed limit.");

        // -------- IMAGE URL --------
        RuleFor(x => x.ImageUrl)
            .Must(BeValidImageUrl).When(x => !string.IsNullOrEmpty(x.ImageUrl))
            .WithMessage("Invalid image URL (must end with .jpg, .jpeg, .png, .gif, or .webp).");

        // -------- BUSINESS RULES --------
        RuleFor(x => x)
            .MustAsync(PassBusinessRules)
            .WithMessage("Product failed business rule validation.");

        // -------- CONDITIONAL VALIDATION --------

        // Electronics
        When(x => x.Category == ProductCategory.Electronics, () =>
        {
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(50)
                .WithMessage("Electronics must have a minimum price of $50.");

            RuleFor(x => x.ReleaseDate)
                .GreaterThan(DateTime.UtcNow.AddYears(-5))
                .WithMessage("Electronics must be released within the last 5 years.");

            RuleFor(x => x.Name)
                .Must(ContainTechnologyKeywords)
                .WithMessage("Electronics name must contain technology-related keywords.");
        });

        // Home
        When(x => x.Category == ProductCategory.Home, () =>
        {
            RuleFor(x => x.Price)
                .LessThanOrEqualTo(200)
                .WithMessage("Home products must have a maximum price of $200.");

            RuleFor(x => x.Name)
                .Must(BeAppropriateForHome)
                .WithMessage("Home product names must be appropriate (no banned terms).");
        });

        // Clothing
        When(x => x.Category == ProductCategory.Clothing, () =>
        {
            RuleFor(x => x.Brand.Length)
                .GreaterThanOrEqualTo(3)
                .WithMessage("Clothing brand name must be at least 3 characters long.");
        });

        // Cross-field rule: Expensive products must have limited stock
        RuleFor(x => x)
            .Must(p => p.Price <= 100 || p.StockQuantity <= 20)
            .WithMessage("Expensive products (> $100) must have limited stock (≤ 20).");
    }

    // ----------- VALIDATION METHODS -----------

    private bool BeValidName(string name)
    {
        var badWords = new[] { "fake", "illegal", "banned" };
        return !badWords.Any(w => name.Contains(w, StringComparison.OrdinalIgnoreCase));
    }

    private Task<bool> BeUniqueName(CreateProductProfileRequest req, string name, CancellationToken token)
    {
        _logger.LogInformation("Validating name uniqueness for {Name} ({Brand})", name, req.Brand);

        var exists = _existingProducts.Any(p =>
            p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
            p.Brand.Equals(req.Brand, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(!exists);
    }

    private Task<bool> BeUniqueSKU(string sku, CancellationToken token)
    {
        _logger.LogInformation("Validating SKU uniqueness for {SKU}", sku);

        var exists = _existingProducts.Any(p =>
            p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(!exists);
    }

    private bool BeValidImageUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
               (url.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".webp", StringComparison.OrdinalIgnoreCase));
    }

    private Task<bool> PassBusinessRules(CreateProductProfileRequest req, CancellationToken token)
    {
        // Rule 1: Max 500 products/day
        if (_existingProducts.Count >= 500)
            return Task.FromResult(false);

        // Rule 2: Electronics min price
        if (req.Category == ProductCategory.Electronics && req.Price < 50)
            return Task.FromResult(false);

        // Rule 3: Home restricted names
        if (req.Category == ProductCategory.Home && req.Name.Contains("weapon", StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(false);

        // Rule 4: High-value stock limit
        if (req.Price > 500 && req.StockQuantity > 10)
            return Task.FromResult(false);

        return Task.FromResult(true);
    }

    // ----------- HELPER METHODS (TASK 3.3) -----------

    private bool ContainTechnologyKeywords(string name)
    {
        var keywords = new[] { "tech", "smart", "AI", "digital", "pro", "ultra", "max" };
        return keywords.Any(k => name.Contains(k, StringComparison.OrdinalIgnoreCase));
    }

    private bool BeAppropriateForHome(string name)
    {
        var banned = new[] { "gun", "explosive", "toxic", "poison" };
        return !banned.Any(b => name.Contains(b, StringComparison.OrdinalIgnoreCase));
    }
}
