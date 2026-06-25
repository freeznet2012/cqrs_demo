using MediatR;

namespace CqrsLearning.MediatR.Api.Features.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDetailsDto?>;
