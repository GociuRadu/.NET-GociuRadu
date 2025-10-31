using AutoMapper;
using Labb5.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Labb5;

public class CreateProductHandler
{
    private readonly ILogger<CreateProductHandler> _logger;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;

    // Simulăm o bază de date în memorie
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
        // 1️⃣ Logging de început
        _logger.LogInformation("Product creation started for {Name} ({SKU}) in category {Category}",
            request.Name, request.SKU, request.Category);

        // 2️⃣ Validare simplă: SKU unic
        if (_products.Any(p => p.SKU.Equals(request.SKU, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Product with SKU {SKU} already exists", request.SKU);
            throw new InvalidOperationException($"Product with SKU '{request.SKU}' already exists.");
        }

        // 3️⃣ Mapare request -> entity
        var product = _mapper.Map<Product>(request);

        // 4️⃣ Salvare (simulată)
        _products.Add(product);

        // 5️⃣ Invalidare cache
        _cache.Remove("all_products");

        // 6️⃣ Mapare entity -> DTO răspuns
        var dto = _mapper.Map<ProductProfileDto>(product);

        // 7️⃣ Logging final
        _logger.LogInformation("Product created successfully: {Name} ({SKU}), price {Price:C2}",
            dto.Name, dto.SKU, dto.Price);

        return dto;
    }

    // helper pentru testare (opțional)
    public IReadOnlyList<Product> GetAll() => _products.AsReadOnly();
}