namespace InventoryPortfolio.Models;

public sealed class Order
{
    public int Id { get; set; }

    // 注文対象の商品
    public int ProductId { get; set; }

    // 受注数量
    public int Quantity { get; set; }

    // キャンセル済みかどうか
    public bool IsCancelled { get; set; }

    // 受注日時（UTC）
    public DateTimeOffset CreatedAt { get; set; }
        = DateTimeOffset.UtcNow;

    // ナビゲーションプロパティ
    public Product Product { get; set; } = null!;
}