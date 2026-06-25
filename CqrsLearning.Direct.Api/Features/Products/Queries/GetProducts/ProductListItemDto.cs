namespace CqrsLearning.Direct.Api.Features.Products.Queries.GetProducts;

public sealed record ProductListItemDto(
    Guid Id,
    string Name,
    decimal Price,
    DateTime CreatedAtUtc);
