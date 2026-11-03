using InventoryPortfolio.Data;
using InventoryPortfolio.Dtos;
using InventoryPortfolio.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryPortfolio.Services;

public sealed class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 受注取得
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<OrderDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrderDto(
                o.Id,
                o.ProductId,
                o.Quantity,
                o.IsCancelled,
                o.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 受注登録
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task<CreateOrderResult> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.ProductId <= 0 || request.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(request),
                "商品IDと数量は1以上で指定してください。");
        }

        // トランザクション開始
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        // 在庫が足りる場合だけ在庫を減らす
        int updated = await _context.Products
            .Where(p =>
                p.Id == request.ProductId &&
                p.StockQuantity >= request.Quantity)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    p => p.StockQuantity,
                    p => p.StockQuantity - request.Quantity),
                cancellationToken);

        if (updated == 0)
        {
            await transaction.RollbackAsync(cancellationToken);

            // 商品が存在するか確認
            bool exists = await _context.Products
                .AnyAsync(
                    p => p.Id == request.ProductId,
                    cancellationToken);

            return new CreateOrderResult(
                null,
                exists
                    ? OrderError.InsufficientStock
                    : OrderError.ProductNotFound);
        }

        // 受注データ作成
        var order = new Order
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            IsCancelled = false,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.Orders.Add(order);

        await _context.SaveChangesAsync(cancellationToken);

        // 在庫更新と受注登録を確定
        await transaction.CommitAsync(cancellationToken);

        var dto = new OrderDto(
            order.Id,
            order.ProductId,
            order.Quantity,
            order.IsCancelled,
            order.CreatedAt);

        return new CreateOrderResult(
            dto,
            OrderError.None);
    }

    /// <summary>
    /// 受注キャンセル
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<OrderError> CancelAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        // 受注情報取得
        var order = await _context.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new
            {
                o.ProductId,
                o.Quantity,
                o.IsCancelled
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null)
        {
            return OrderError.OrderNotFound;
        }

        if (order.IsCancelled)
        {
            return OrderError.AlreadyCancelled;
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        // 未キャンセルの場合だけ状態を更新
        int updated = await _context.Orders
            .Where(o => o.Id == id && !o.IsCancelled)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    o => o.IsCancelled,
                    true),
                cancellationToken);

        if (updated == 0)
        {
            return OrderError.AlreadyCancelled;
        }

        // 在庫を戻す
        int restored = await _context.Products
            .Where(p => p.Id == order.ProductId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    p => p.StockQuantity,
                    p => p.StockQuantity + order.Quantity),
                cancellationToken);

        if (restored != 1)
        {
            throw new InvalidOperationException(
                "在庫の復元に失敗しました。");
        }

        await transaction.CommitAsync(cancellationToken);

        return OrderError.None;
    }
}