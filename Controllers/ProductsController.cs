using InventoryPortfolio.Dtos;
using InventoryPortfolio.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryPortfolio.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET api/products
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>>
        GetAll(CancellationToken cancellationToken)
    {
        var products = await _productService
            .GetAllAsync(cancellationToken);

        return Ok(products);
    }

    // GET api/products/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await _productService
            .GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    // POST api/products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _productService
            .CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    // PUT api/products/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _productService
            .UpdateAsync(id, request, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }
}