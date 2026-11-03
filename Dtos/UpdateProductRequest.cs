using System.ComponentModel.DataAnnotations;

namespace InventoryPortfolio.Dtos;

public sealed class UpdateProductRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}