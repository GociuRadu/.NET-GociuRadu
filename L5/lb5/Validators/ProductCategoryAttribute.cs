using System.ComponentModel.DataAnnotations;
using Labb5.Products;

namespace Labb5.Validators.Attributes;

public class ProductCategoryAttribute : ValidationAttribute
{
    private readonly ProductCategory[] _allowed;

    public ProductCategoryAttribute(params ProductCategory[] allowed)
    {
        _allowed = allowed;
    }

    public override bool IsValid(object? value)
    {
        if (value is not ProductCategory category) return false;
        return _allowed.Contains(category);
    }

    public override string FormatErrorMessage(string name)
        => $"Category must be one of: {string.Join(", ", _allowed)}";
}