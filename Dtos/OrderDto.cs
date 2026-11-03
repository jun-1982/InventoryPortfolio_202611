namespace InventoryPortfolio.Dtos;

public sealed record OrderDto(
    int Id,
    int ProductId,
    int Quantity,
    bool IsCancelled,
    DateTimeOffset CreatedAt);