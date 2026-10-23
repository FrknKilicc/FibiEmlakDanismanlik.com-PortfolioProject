namespace FibiEmlakDanismanlik.AdminUI.Infrastructure;

/// <summary>
/// WebApi çağrılarından dönen sonuçları sarmalayan model.
/// </summary>
/// <typeparam name="T">Dönen verinin tipi.</typeparam>
public sealed class ApiResult<T>
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public T? Data { get; init; }
    public string? ErrorMessage { get; init; }

    public static ApiResult<T> Ok(T? data, int statusCode = 200) =>
        new() { Success = true, StatusCode = statusCode, Data = data };

    public static ApiResult<T> Fail(string? errorMessage, int statusCode, T? data = default) =>
        new() { Success = false, StatusCode = statusCode, ErrorMessage = errorMessage, Data = data };
}
