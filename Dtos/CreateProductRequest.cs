using System.ComponentModel.DataAnnotations;

namespace InventoryPortfolio.Dtos;

public sealed class CreateProductRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
}