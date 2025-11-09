using System.Net.Http.Json;
using CashControl.IntegrationTests.Models;

namespace CashControl.IntegrationTests.Extensions;

public static class HttpResponseExtensions
{
    public static async Task<Result<TData>?> ReadAsResultAsync<TData>(
        this HttpResponseMessage response
    ) => await response.Content.ReadFromJsonAsync<Result<TData>>();
}
