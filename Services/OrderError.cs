namespace InventoryPortfolio.Services;

public enum OrderError
{
    None,
    ProductNotFound,
    InsufficientStock,
    OrderNotFound,
    AlreadyCancelled
}