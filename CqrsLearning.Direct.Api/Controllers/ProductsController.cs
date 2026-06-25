using CqrsLearning.Direct.Api.Features.Products.Commands.CreateProduct;
using CqrsLearning.Direct.Api.Features.Products.Commands.DeactivateProduct;
using CqrsLearning.Direct.Api.Features.Products.Commands.UpdateProductPrice;
using CqrsLearning.Direct.Api.Features.Products.Queries.GetProductById;
using CqrsLearning.Direct.Api.Features.Products.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;

namespace CqrsLearning.Direct.Api.Controllers;

[ApiController]
[Route("products")]
public sealed class ProductsController : ControllerBase
{
    private readonly CreateProductCommandHandler _createProductCommandHandler;
    private readonly DeactivateProductCommandHandler _deactivateProductCommandHandler;
    private readonly GetProductByIdQueryHandler _getProductByIdQueryHandler;
    private readonly GetProductsQueryHandler _getProductsQueryHandler;
    private readonly UpdateProductPriceCommandHandler _updateProductPriceCommandHandler;

    public ProductsController(
        CreateProductCommandHandler createProductCommandHandler,
        DeactivateProductCommandHandler deactivateProductCommandHandler,
        GetProductByIdQueryHandler getProductByIdQueryHandler,
        GetProductsQueryHandler getProductsQueryHandler,
        UpdateProductPriceCommandHandler updateProductPriceCommandHandler)
    {
        _createProductCommandHandler = createProductCommandHandler;
        _deactivateProductCommandHandler = deactivateProductCommandHandler;
        _getProductByIdQueryHandler = getProductByIdQueryHandler;
        _getProductsQueryHandler = getProductsQueryHandler;
        _updateProductPriceCommandHandler = updateProductPriceCommandHandler;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductListItemDto>>> GetProducts(
        CancellationToken cancellationToken)
    {
        var products = await _getProductsQueryHandler.Handle(
            new GetProductsQuery(),
            cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDetailsDto>> GetProductById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _getProductByIdQueryHandler.Handle(
            new GetProductByIdQuery(id),
            cancellationToken);

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
        var result = await _createProductCommandHandler.Handle(command, cancellationToken);

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
        var result = await _updateProductPriceCommandHandler.Handle(
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
        var result = await _deactivateProductCommandHandler.Handle(
            new DeactivateProductCommand(id),
            cancellationToken);

        if (result.NotFound)
        {
            return NotFound();
        }

        return NoContent();
    }
}
