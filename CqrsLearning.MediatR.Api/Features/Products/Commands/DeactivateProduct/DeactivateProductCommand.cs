using MediatR;

namespace CqrsLearning.MediatR.Api.Features.Products.Commands.DeactivateProduct;

public sealed record DeactivateProductCommand(Guid ProductId) : IRequest<DeactivateProductResult>;
