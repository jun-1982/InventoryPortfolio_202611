using InventoryPortfolio.Data;
using InventoryPortfolio.Dtos;
using InventoryPortfolio.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryPortfolio.Services;

public sealed class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 商品一覧取得
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.StockQuantity))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// 商品IDによる取得
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.StockQuantity))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 商品登録
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductDto> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Name = request.Name.Trim(),
            StockQuantity = request.StockQuantity
        };

        _context.Products.Add(product);

        await _context.SaveChangesAsync(cancellationToken);

        return new ProductDto(
            product.Id,
            product.Name,
            product.StockQuantity);
    }

    /// <summary>
    /// 商品名変更
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(
                p => p.Id == id,
                cancellationToken);

        if (product is null)
        {
            return false;
        }

        product.Name = request.Name.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}