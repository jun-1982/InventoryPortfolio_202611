using InventoryPortfolio.Dtos;
using InventoryPortfolio.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventoryPortfolio.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET api/orders/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var order = await _orderService
            .GetByIdAsync(id, cancellationToken);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    // POST api/orders
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _orderService
            .CreateAsync(request, cancellationToken);

        if (result.Error == OrderError.ProductNotFound)
        {
            return NotFound(new
            {
                message = "商品が存在しません。"
            });
        }

        if (result.Error == OrderError.InsufficientStock)
        {
            return Conflict(new
            {
                message = "在庫が不足しています。"
            });
        }

        if (result.Order is null)
        {
            throw new InvalidOperationException(
                "受注登録結果が不正です。");
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Order.Id },
            result.Order);
    }

    // POST api/orders/1/cancel
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _orderService
            .CancelAsync(id, cancellationToken);

        return result switch
        {
            OrderError.None => NoContent(),

            OrderError.OrderNotFound => NotFound(),

            OrderError.AlreadyCancelled => Conflict(
                new { message = "キャンセル済みです。" }),

            _ => StatusCode(500)
        };
    }
}