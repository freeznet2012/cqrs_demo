namespace CqrsLearning.MediatR.Api.Features.Products.Queries.GetProductById;

public sealed record ProductDetailsDto(
    Guid Id,
    string Name,
    decimal Price,
    DateTime CreatedAtUtc);
