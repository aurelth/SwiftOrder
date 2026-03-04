using MediatR;
using Microsoft.AspNetCore.Mvc;
using SwiftOrder.Application.DTOs.Orders;
using SwiftOrder.Application.UseCases.Orders.AddOrderItem;
using SwiftOrder.Application.UseCases.Orders.CreateOrder;
using SwiftOrder.Application.UseCases.Orders.GetOrderById;
using SwiftOrder.Application.UseCases.Orders.SubmitOrder;

namespace SwiftOrder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateOrderCommand(request), ct);
        return Ok(id);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<ActionResult<OrderDetailsDto>> GetById(Guid orderId, CancellationToken ct)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(orderId), ct);
        return Ok(order);
    }

    [HttpPost("{orderId:guid}/items")]
    public async Task<IActionResult> AddItem(Guid orderId, [FromBody] AddOrderItemRequest request, CancellationToken ct)
    {
        await _mediator.Send(new AddOrderItemCommand(orderId, request), ct);
        return NoContent();
    }

    [HttpPost("{orderId:guid}/submit")]
    public async Task<IActionResult> Submit(Guid orderId, CancellationToken ct)
    {
        await _mediator.Send(new SubmitOrderCommand(orderId), ct);
        return NoContent();
    }
}