using System.Net.Http.Json;
using CashControl.IntegrationTests.Models;

namespace CashControl.IntegrationTests.Extensions;

public static class HttpResponseExtensions
{
    public static Result<TData>? ReadAsResultAsync<TData>(this HttpResponseMessage response) =>
        response.Content.ReadFromJsonAsync<Result<TData>>().Result;
}
