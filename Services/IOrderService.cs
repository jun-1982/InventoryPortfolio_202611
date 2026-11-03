using InventoryPortfolio.Dtos;

namespace InventoryPortfolio.Services;

public interface IOrderService
{
    /// <summary>
    /// 受注取得
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OrderDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 受注登録
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CreateOrderResult> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 受注キャンセル
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<OrderError> CancelAsync(
        int id,
        CancellationToken cancellationToken = default);
}