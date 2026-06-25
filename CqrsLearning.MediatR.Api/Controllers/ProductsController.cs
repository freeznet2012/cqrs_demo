using CqrsLearning.MediatR.Api.Features.Products.Commands.CreateProduct;
using CqrsLearning.MediatR.Api.Features.Products.Commands.DeactivateProduct;
using CqrsLearning.MediatR.Api.Features.Products.Commands.UpdateProductPrice;
using CqrsLearning.MediatR.Api.Features.Products.Queries.GetProductById;
using CqrsLearning.MediatR.Api.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CqrsLearning.MediatR.Api.Controllers;

[ApiController]
[Route("products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductListItemDto>>> GetProducts(
        CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(new GetProductsQuery(), cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDetailsDto>> GetProductById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<CreateProductResponse>> CreateProduct(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return ValidationProblem(new ValidationProblemDetails(result.Errors));
        }

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = result.Response!.Id },
            result.Response);
    }

    [HttpPut("{id:guid}/price")]
    public async Task<IActionResult> UpdateProductPrice(
        Guid id,
        UpdateProductPriceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateProductPriceCommand(id, request.Price),
            cancellationToken);

        if (result.NotFound)
        {
            return NotFound();
        }

        if (!result.IsSuccess)
        {
            return ValidationProblem(new ValidationProblemDetails(result.Errors));
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateProduct(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeactivateProductCommand(id),
            cancellationToken);

        if (result.NotFound)
        {
            return NotFound();
        }

        return NoContent();
    }
}
