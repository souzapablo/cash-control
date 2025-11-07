namespace CashControl.Api.Abstractions;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}
