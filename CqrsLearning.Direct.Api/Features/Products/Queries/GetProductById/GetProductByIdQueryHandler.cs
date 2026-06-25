using CqrsLearning.Direct.Api.Application.Abstractions;
using CqrsLearning.Direct.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CqrsLearning.Direct.Api.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler
    : IQueryHandler<GetProductByIdQuery, ProductDetailsDto?>
{
    private readonly AppDbContext _dbContext;

    public GetProductByIdQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProductDetailsDto?> Handle(
        GetProductByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(product => product.Id == query.Id && product.IsActive)
            .Select(product => new ProductDetailsDto(
                product.Id,
                product.Name,
                product.Price,
                product.CreatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
