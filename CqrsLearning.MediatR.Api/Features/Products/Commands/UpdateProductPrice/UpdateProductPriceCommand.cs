using MediatR;

namespace CqrsLearning.MediatR.Api.Features.Products.Commands.UpdateProductPrice;

public sealed record UpdateProductPriceCommand(
    Guid ProductId,
    decimal Price) : IRequest<UpdateProductPriceResult>;
