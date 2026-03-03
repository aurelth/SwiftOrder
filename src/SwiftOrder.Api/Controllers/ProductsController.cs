using MediatR;
using Microsoft.AspNetCore.Mvc;
using SwiftOrder.Application.DTOs.Products;
using SwiftOrder.Application.UseCases.Products.CreateProduct;
using SwiftOrder.Application.UseCases.Products.GetProducts;

namespace SwiftOrder.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> Get(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductsQuery(), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateProductCommand(request), ct);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }
}