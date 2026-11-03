using InventoryPortfolio.Dtos;

namespace InventoryPortfolio.Services;

public sealed record CreateOrderResult(
    OrderDto? Order,
    OrderError Error);