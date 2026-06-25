using MediatR;

namespace CqrsLearning.MediatR.Api.Features.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    decimal Price) : IRequest<CreateProductResult>;
