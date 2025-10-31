using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.RegularExpressions;

namespace Labb5.Validators.Attributes;

public class ValidSKUAttribute : ValidationAttribute, IClientModelValidator
{
    public override bool IsValid(object? value)
    {
        if (value == null) return true;
        var sku = value.ToString()?.Replace(" ", "");
        return Regex.IsMatch(sku ?? "", @"^[A-Za-z0-9\-]{5,20}$");
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes["data-val"] = "true";
        context.Attributes["data-val-validsku"] = ErrorMessage ??
                                                  "SKU must be alphanumeric, 5-20 characters, with hyphens allowed.";
    }
}