using InventoryPortfolio.Dtos;

namespace InventoryPortfolio.Services;

public interface IProductService
{
    // 商品一覧取得
    Task<IReadOnlyList<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    // 商品IDによる取得
    Task<ProductDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    // 商品登録
    Task<ProductDto> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default);

    // 商品名変更
    Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default);
}