using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FibiEmlakDanismanlik.AdminUI.Infrastructure;

/// <summary>
/// HTML5 form alanlarından (type="number") tarayıcı tarafından nokta ayraçlı ("1500.5", "41.0123") gönderilen
/// ondalıklı sayıların, sunucu kültürü (tr-TR) altında nokta binlik ayracı sayıldığı için ("15005") yanlış
/// okunmasını önleyen model bağlayıcı.
/// </summary>
public sealed class InvariantDecimalModelBinder : IModelBinder
{
    private static readonly CultureInfo _turkishCulture = CultureInfo.GetCultureInfo("tr-TR");

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var modelName = bindingContext.ModelName;
        var valueResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueResult);

        var rawValue = valueResult.FirstValue;
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            if (bindingContext.ModelMetadata.IsReferenceOrNullableType)
            {
                bindingContext.Result = ModelBindingResult.Success(null);
            }

            return Task.CompletedTask;
        }

        var trimmed = rawValue.Trim();
        var targetType = bindingContext.ModelMetadata.UnderlyingOrModelType;
        bool isSuccess = false;
        object? parsedValue = null;

        if (targetType == typeof(decimal))
        {
            if (decimal.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var dec))
            {
                parsedValue = dec;
                isSuccess = true;
            }
            else if (trimmed.Contains(',') && decimal.TryParse(trimmed, NumberStyles.Float | NumberStyles.AllowThousands, _turkishCulture, out var decTr))
            {
                parsedValue = decTr;
                isSuccess = true;
            }
        }
        else if (targetType == typeof(double))
        {
            if (double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var dbl))
            {
                parsedValue = dbl;
                isSuccess = true;
            }
            else if (trimmed.Contains(',') && double.TryParse(trimmed, NumberStyles.Float | NumberStyles.AllowThousands, _turkishCulture, out var dblTr))
            {
                parsedValue = dblTr;
                isSuccess = true;
            }
        }

        if (isSuccess)
        {
            bindingContext.Result = ModelBindingResult.Success(parsedValue);
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(modelName, "Geçerli bir sayı girin.");
            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// <see cref="decimal"/> ve <see cref="double"/> (ve bunlara karşılık gelen boş değer alabilen türler) için
/// <see cref="InvariantDecimalModelBinder"/> sağlayan model bağlayıcı sağlayıcısı.
/// </summary>
public sealed class InvariantDecimalModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var targetType = context.Metadata.UnderlyingOrModelType;
        if (targetType == typeof(decimal) || targetType == typeof(double))
        {
            return new InvariantDecimalModelBinder();
        }

        return null;
    }
}
