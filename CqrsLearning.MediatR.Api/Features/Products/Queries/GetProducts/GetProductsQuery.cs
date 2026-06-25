using MediatR;

namespace CqrsLearning.MediatR.Api.Features.Products.Queries.GetProducts;

public sealed record GetProductsQuery : IRequest<IReadOnlyList<ProductListItemDto>>;
