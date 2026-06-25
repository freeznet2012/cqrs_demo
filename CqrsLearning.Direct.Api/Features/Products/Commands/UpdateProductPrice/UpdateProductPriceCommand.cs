namespace CqrsLearning.Direct.Api.Features.Products.Commands.UpdateProductPrice;

public sealed record UpdateProductPriceCommand(
    Guid ProductId,
    decimal Price);
