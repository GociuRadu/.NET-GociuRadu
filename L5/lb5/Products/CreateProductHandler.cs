using AutoMapper;
using Labb5.DTOs;
using Labb5.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Labb5.Products;

public class CreateProductHandler
{
    private readonly ILogger<CreateProductHandler> _logger;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;

    private static readonly List<Product> _products = new();

    public CreateProductHandler(
        ILogger<CreateProductHandler> logger,
        IMemoryCache cache,
        IMapper mapper)
    {
        _logger = logger;
        _cache = cache;
        _mapper = mapper;
    }

    public ProductProfileDto Handle(CreateProductProfileRequest request)
    {
        var operationId = Guid.NewGuid().ToString("N")[..8]; // scurt pentru identificare
        var totalTimer = Stopwatch.StartNew();

        using var scope = _logger.BeginScope("ProductOperation {OperationId}", operationId);
        _logger.LogInformation(LogEvents.ProductCreationStarted,
            "Starting product creation for {Name} ({SKU}) in category {Category}",
            request.Name, request.SKU, request.Category);

        // ---------- VALIDATION ----------
        var validationTimer = Stopwatch.StartNew();
        try
        {
            _logger.LogInformation(LogEvents.SKUValidationPerformed,
                "Validating SKU {SKU}", request.SKU);

            if (_products.Any(p => p.SKU.Equals(request.SKU, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning(LogEvents.ProductValidationFailed,
                    "Duplicate SKU {SKU} detected", request.SKU);
                throw new InvalidOperationException($"Product with SKU '{request.SKU}' already exists.");
            }

            _logger.LogInformation(LogEvents.StockValidationPerformed,
                "Stock validation performed: {StockQuantity}", request.StockQuantity);
        }
        catch (Exception ex)
        {
            _logger.LogError(LogEvents.ProductValidationFailed, ex, "Validation failed for {SKU}", request.SKU);
            throw;
        }
        validationTimer.Stop();

        // ---------- DATABASE ----------
        var dbTimer = Stopwatch.StartNew();
        _logger.LogInformation(LogEvents.DatabaseOperationStarted,
            "Saving product {Name} ({SKU}) to database", request.Name, request.SKU);

        var product = _mapper.Map<Product>(request);
        _products.Add(product);

        dbTimer.Stop();
        _logger.LogInformation(LogEvents.DatabaseOperationCompleted,
            "Database save completed for {SKU} (Duration: {DbTime} ms)",
            request.SKU, dbTimer.ElapsedMilliseconds);

        // ---------- CACHE ----------
        _cache.Remove("all_products");
        _logger.LogInformation(LogEvents.CacheOperationPerformed,
            "Cache invalidated for key 'all_products'");

        totalTimer.Stop();

        // ---------- METRICS ----------
        var metrics = new ProductCreationMetrics(
            OperationId: operationId,
            ProductName: product.Name,
            SKU: product.SKU,
            Category: product.Category,
            ValidationDuration: validationTimer.Elapsed,
            DatabaseSaveDuration: dbTimer.Elapsed,
            TotalDuration: totalTimer.Elapsed,
            Success: true
        );

        _logger.LogInformation(LogEvents.ProductCreationCompleted,
            "Product created successfully in {Total} ms. Metrics: {@Metrics}",
            totalTimer.ElapsedMilliseconds, metrics);

        return _mapper.Map<ProductProfileDto>(product);
    }

    public IReadOnlyList<Product> GetAll() => _products.AsReadOnly();
}
