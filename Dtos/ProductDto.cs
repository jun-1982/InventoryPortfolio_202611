namespace InventoryPortfolio.Dtos
{
    public sealed record ProductDto(
        int Id,
        string Name,
        int StockQuantity);
}
